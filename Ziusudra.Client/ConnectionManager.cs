using System.Net;
using Ziusudra.DelugeRpc;
using Ziusudra.DelugeRpc.Daemon;

namespace Ziusudra.Client
{

    /// <summary>Manages the saved host list and drives a <see cref="DelugeSession" /> for the selected host.</summary>
    /// <remarks>This is the UI-agnostic heart of the connection manager: it owns host-list persistence and the
    /// resolve-then-connect flow, so it is unit-testable without a UI thread. The Desktop view-model is a thin wrapper.</remarks>
    public sealed class ConnectionManager
    {

        /// <summary>Create a new instance of the <see cref="ConnectionManager" /> type.</summary>
        /// <param name="session">The session driven for the selected host.</param>
        /// <param name="store">The host-list store.</param>
        /// <param name="resolver">The host resolver.</param>
        /// <param name="clientFactory">Creates the transient <see cref="IRpcClient" /> used to probe a host's status.</param>
        public ConnectionManager(DelugeSession session, IHostStore store, IHostResolver resolver, Func<IPEndPoint, IRpcClient> clientFactory)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(resolver);
            ArgumentNullException.ThrowIfNull(clientFactory);

            _Session = session;
            _Store = store;
            _Resolver = resolver;
            _ClientFactory = clientFactory;
            _Session.StateChanged += OnSessionStateChanged;
        }

        /// <summary>Loads the saved host list.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<HostEntry> entries = await _Store.LoadAsync(cancellationToken)
                .ConfigureAwait(false);
            _Hosts.Clear();
            _Hosts.AddRange(entries);
        }

        /// <summary>Adds the specified <paramref name="entry" /> to the host list and persists it.</summary>
        /// <param name="entry">The entry to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AddAsync(HostEntry entry, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);
            _Hosts.Add(entry);
            await _Store.SaveAsync(_Hosts, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>Replaces the stored entry matching <paramref name="entry" />'s identifier and persists the change.</summary>
        /// <param name="entry">The entry carrying the updated values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateAsync(HostEntry entry, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);
            int index = _Hosts.FindIndex(h => h.Id == entry.Id);
            if (index < 0)
                return;

            _Hosts[index] = entry;
            await _Store.SaveAsync(_Hosts, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>Removes the entry matching <paramref name="entry" /> from the host list and persists the change.</summary>
        /// <param name="entry">The entry to remove.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task RemoveAsync(HostEntry entry, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);
            _Hosts.RemoveAll(h => h.Id == entry.Id);
            await _Store.SaveAsync(_Hosts, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>Resolves the specified host <paramref name="entry" /> and connects the session to it.</summary>
        /// <param name="entry">The host to connect to.</param>
        /// <param name="password">The account password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task ConnectAsync(HostEntry entry, string password, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);
            ArgumentNullException.ThrowIfNull(password);

            IPEndPoint endpoint = await _Resolver.ResolveAsync(entry.Host, entry.Port, cancellationToken)
                .ConfigureAwait(false);
            await _Session.ConnectAsync(endpoint, entry.Username, password, cancellationToken)
                .ConfigureAwait(false);
            _ConnectedHostId = entry.Id;
            HostStatusChanged?.Invoke(this, new HostStatusChangedEventArgs(entry.Id, HostStatus.Connected, _Session.Capabilities?.DaemonVersion ?? string.Empty));
        }

        /// <summary>Disconnects the session.</summary>
        public Task DisconnectAsync()
        {
            return _Session.DisconnectAsync();
        }

        /// <summary>Probes every saved host's reachability in parallel, raising <see cref="HostStatusChanged" /> as each completes.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task RefreshStatusAsync(CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(_Hosts.ToArray().Select(entry => ProbeAndRaiseAsync(entry, cancellationToken)));
        }

        private async Task ProbeAndRaiseAsync(HostEntry entry, CancellationToken cancellationToken)
        {
            (HostStatus status, string version) = await ProbeAsync(entry, cancellationToken)
                .ConfigureAwait(false);
            HostStatusChanged?.Invoke(this, new HostStatusChangedEventArgs(entry.Id, status, version));
        }

        private async Task<(HostStatus Status, string Version)> ProbeAsync(HostEntry entry, CancellationToken cancellationToken)
        {
            if (_Session.State == SessionState.Connected && entry.Id == _ConnectedHostId)
                return (HostStatus.Connected, _Session.Capabilities?.DaemonVersion ?? string.Empty);

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(ProbeTimeout);
            try
            {
                IPEndPoint endpoint = await _Resolver.ResolveAsync(entry.Host, entry.Port, timeout.Token)
                    .ConfigureAwait(false);
                IRpcClient client = _ClientFactory(endpoint);
                try
                {
                    await client.StartAsync(timeout.Token)
                        .ConfigureAwait(false);
                    InfoRequest.Response info = await client.SendRequestAsync(new InfoRequest(), timeout.Token)
                        .ConfigureAwait(false);
                    return (HostStatus.Online, info.Version);
                } finally
                {
                    await client.StopAsync()
                        .ConfigureAwait(false);
                }
            } catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
#pragma warning disable CA1031 // Any probe failure (unresolved, unreachable, timeout, TLS) means the host is offline.
            catch (Exception)
            {
                return (HostStatus.Offline, string.Empty);
            }
#pragma warning restore CA1031
        }

        private void OnSessionStateChanged(object? sender, EventArgs e)
        {
            if (_Session.State is SessionState.Disconnected or SessionState.Faulted)
                _ConnectedHostId = null;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Gets the saved host list.</summary>
        public IReadOnlyList<HostEntry> Hosts => _Hosts;

        /// <summary>Gets the current session state.</summary>
        public SessionState State => _Session.State;

        /// <summary>Gets the capabilities of the connected daemon, or <c>null</c> while not connected.</summary>
        public ServerCapabilities? Capabilities => _Session.Capabilities;

        /// <summary>Raised after every transition of <see cref="State" />.</summary>
        public event EventHandler? StateChanged;

        /// <summary>Raised as each host's status is resolved by <see cref="RefreshStatusAsync" /> or on connect.</summary>
        public event EventHandler<HostStatusChangedEventArgs>? HostStatusChanged;

        private static readonly TimeSpan ProbeTimeout = TimeSpan.FromSeconds(4);

        private readonly List<HostEntry> _Hosts = new();
        private readonly IHostResolver _Resolver;
        private readonly DelugeSession _Session;
        private readonly IHostStore _Store;
        private readonly Func<IPEndPoint, IRpcClient> _ClientFactory;
        private Guid? _ConnectedHostId;
    }
}
