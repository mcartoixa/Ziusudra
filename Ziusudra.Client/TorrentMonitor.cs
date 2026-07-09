using Ziusudra.DelugeRpc;
using Ziusudra.DelugeRpc.Core;
using Ziusudra.DelugeRpc.Events;

namespace Ziusudra.Client
{

    /// <summary>Maintains an observable snapshot of the daemon's torrents by polling and reacting to server events.</summary>
    /// <remarks>A steady poll of <c>core.get_torrents_status</c> is the baseline cadence; server events keep the model
    /// responsive between polls. Results are diffed by torrent hash into the model, and only real changes raise
    /// notifications — the diff compares <b>by value</b>, never by type, because rencode round-trips to the most compact
    /// CLR type. Like <see cref="DelugeSession" />, the monitor is thread-agnostic: it raises notifications on whichever
    /// thread produced them, and consumers marshal onto a UI thread when projecting into bound collections.</remarks>
    public sealed class TorrentMonitor:
        IAsyncDisposable
    {

        /// <summary>Create a new instance of the <see cref="TorrentMonitor" /> type polling at the default cadence.</summary>
        /// <param name="client">The client used to poll the daemon and receive its events.</param>
        /// <param name="statusKeys">The status fields to request for each torrent.</param>
        public TorrentMonitor(IRpcClient client, IEnumerable<string> statusKeys):
            this(client, statusKeys, DefaultPollInterval)
        { }

        /// <summary>Create a new instance of the <see cref="TorrentMonitor" /> type.</summary>
        /// <param name="client">The client used to poll the daemon and receive its events.</param>
        /// <param name="statusKeys">The status fields to request for each torrent.</param>
        /// <param name="pollInterval">The steady polling cadence.</param>
        public TorrentMonitor(IRpcClient client, IEnumerable<string> statusKeys, TimeSpan pollInterval)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(statusKeys);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pollInterval, TimeSpan.Zero);

            _Client = client;
            _StatusKeys = statusKeys.ToArray();
            _PollInterval = pollInterval;
            _Client.RpcEventReceived += OnRpcEventReceived;
        }

        /// <summary>Gets the default polling cadence, mirroring the GTK client's ~2 s refresh.</summary>
        public static TimeSpan DefaultPollInterval { get; } = TimeSpan.FromSeconds(2);

        /// <summary>Gets a snapshot of the currently known torrents.</summary>
        public IReadOnlyCollection<Torrent> Torrents
        {
            get
            {
                lock (_Sync)
                    return _Torrents.Values.ToArray();
            }
        }

        /// <summary>Raised when a torrent appears in the model.</summary>
        public event EventHandler<TorrentChangedEventArgs>? TorrentAdded;

        /// <summary>Raised when a known torrent's status changes.</summary>
        public event EventHandler<TorrentChangedEventArgs>? TorrentUpdated;

        /// <summary>Raised when a torrent leaves the model.</summary>
        public event EventHandler<TorrentChangedEventArgs>? TorrentRemoved;

        /// <summary>Raised after each completed poll cycle.</summary>
        public event EventHandler? Refreshed;

        /// <summary>Raised when a poll fails; the connection itself is faulted through <see cref="DelugeSession" />.</summary>
        public event EventHandler<PollFailedEventArgs>? PollFailed;

        /// <summary>Starts the steady polling loop. The first poll happens after one interval.</summary>
        public void Start()
        {
            _LoopTask ??= PollLoopAsync(_Cts.Token);
        }

        /// <summary>Narrows the polled torrents to those matching <paramref name="filter" /> and reconciles the model immediately.</summary>
        /// <remarks>The model then holds only the matching subset: torrents that fall outside the filter are raised through
        /// <see cref="TorrentRemoved" /> and reappear through <see cref="TorrentAdded" /> when the filter widens again.</remarks>
        /// <param name="filter">The filter to narrow by, keyed by filter category. An empty filter clears the narrowing.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SetFilterAsync(IReadOnlyDictionary<string, string> filter, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            lock (_Sync)
                _Filter = filter;
            await RefreshAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>Polls the daemon once and reconciles the result into the model, raising change notifications.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            var added = new List<Torrent>();
            var updated = new List<Torrent>();
            var removed = new List<Torrent>();

            await _RefreshGate.WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            try
            {
                IReadOnlyDictionary<string, string> filter;
                lock (_Sync)
                    filter = _Filter;
                GetTorrentsStatusRequest.Response response = await _Client
                    .SendRequestAsync(new GetTorrentsStatusRequest(filter, _StatusKeys), cancellationToken)
                    .ConfigureAwait(false);
                var incoming = response.Torrents.ToDictionary(t => t.Hash, StringComparer.Ordinal);

                lock (_Sync)
                {
                    foreach (KeyValuePair<string, Torrent> entry in incoming)
                    {
                        if (!_Torrents.TryGetValue(entry.Key, out Torrent? existing))
                            added.Add(entry.Value);
                        else if (!AreEqual(existing, entry.Value))
                            updated.Add(entry.Value);
                    }
                    foreach (KeyValuePair<string, Torrent> entry in _Torrents)
                    {
                        if (!incoming.ContainsKey(entry.Key))
                            removed.Add(entry.Value);
                    }

                    _Torrents.Clear();
                    foreach (KeyValuePair<string, Torrent> entry in incoming)
                        _Torrents[entry.Key] = entry.Value;
                }
            } finally
            {
                _RefreshGate.Release();
            }

            foreach (Torrent torrent in added)
                TorrentAdded?.Invoke(this, new TorrentChangedEventArgs(torrent));
            foreach (Torrent torrent in updated)
                TorrentUpdated?.Invoke(this, new TorrentChangedEventArgs(torrent));
            foreach (Torrent torrent in removed)
                TorrentRemoved?.Invoke(this, new TorrentChangedEventArgs(torrent));
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Stops polling and releases the monitor.</summary>
        public async ValueTask DisposeAsync()
        {
            _Client.RpcEventReceived -= OnRpcEventReceived;
            await _Cts.CancelAsync()
                .ConfigureAwait(false);

            Task? loop = _LoopTask;
            if (loop != null)
            {
                try
                {
                    await loop.ConfigureAwait(false);
                } catch (OperationCanceledException)
                { }
            }

            _Cts.Dispose();
            _RefreshGate.Dispose();
        }

        private async Task PollLoopAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(_PollInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
                    await RunGuardedRefreshAsync(cancellationToken).ConfigureAwait(false);
            } catch (OperationCanceledException)
            { }
        }

        private async Task RunGuardedRefreshAsync(CancellationToken cancellationToken)
        {
            try
            {
                await RefreshAsync(cancellationToken).ConfigureAwait(false);
            } catch (OperationCanceledException)
            {
                throw;
            }
#pragma warning disable CA1031 // A transient poll failure is surfaced through PollFailed rather than crashing the monitor.
            catch (Exception ex)
            {
                PollFailed?.Invoke(this, new PollFailedEventArgs(ex));
            }
#pragma warning restore CA1031
        }

        private void OnRpcEventReceived(object? sender, RpcEventReceivedEventArgs e)
        {
            switch (e.Event)
            {
                case TorrentRemovedEvent removed:
                    RemoveTorrent(removed.TorrentId);
                    break;
                case TorrentAddedEvent:
                case TorrentStateChangedEvent:
                case TorrentQueueChangedEvent:
                case SessionPausedEvent:
                case SessionResumedEvent:
                    _ = RunGuardedRefreshAsync(_Cts.Token);
                    break;
            }
        }

        private void RemoveTorrent(string hash)
        {
            Torrent? removed = null;
            lock (_Sync)
            {
                if (_Torrents.Remove(hash, out Torrent? torrent))
                    removed = torrent;
            }
            if (removed != null)
                TorrentRemoved?.Invoke(this, new TorrentChangedEventArgs(removed));
        }

        private static bool AreEqual(Torrent left, Torrent right)
        {
            return
                string.Equals(left.Hash, right.Hash, StringComparison.Ordinal) &&
                left.ActiveTime == right.ActiveTime &&
                left.DownloadPayloadRate == right.DownloadPayloadRate &&
                left.ExpectedTimeOfArrival == right.ExpectedTimeOfArrival &&
                string.Equals(left.Name, right.Name, StringComparison.Ordinal) &&
                left.Progress == right.Progress &&
                left.Queue == right.Queue &&
                left.SeedingTime == right.SeedingTime &&
                left.State == right.State &&
                left.TotalWanted == right.TotalWanted &&
                left.UploadPayloadRate == right.UploadPayloadRate;
        }

        private readonly IRpcClient _Client;
        private readonly string[] _StatusKeys;
        private readonly TimeSpan _PollInterval;
        private readonly Dictionary<string, Torrent> _Torrents = new(StringComparer.Ordinal);
        private IReadOnlyDictionary<string, string> _Filter = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly object _Sync = new();
        private readonly SemaphoreSlim _RefreshGate = new(1, 1);
        private readonly CancellationTokenSource _Cts = new();
        private Task? _LoopTask;
    }
}
