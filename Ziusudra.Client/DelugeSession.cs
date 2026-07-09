using System.Globalization;
using System.Net;
using Ziusudra.DelugeRpc;
using Ziusudra.DelugeRpc.Core;
using Ziusudra.DelugeRpc.Daemon;

namespace Ziusudra.Client
{

    /// <summary>Orchestrates a single connection to a Deluge daemon over an <see cref="IRpcClient" />.</summary>
    /// <remarks>The session drives the connection lifecycle and the bootstrap handshake that <see cref="IRpcClient" />
    /// deliberately leaves to the caller, and exposes the resolved <see cref="ServerCapabilities" />. It is thread-agnostic:
    /// it raises its notifications on whichever thread produced them, and consumers marshal onto a UI thread when needed.</remarks>
    public sealed class DelugeSession:
        IAsyncDisposable
    {

        /// <summary>Create a new instance of the <see cref="DelugeSession" /> type.</summary>
        /// <param name="clientFactory">Creates the <see cref="IRpcClient" /> for a resolved endpoint. The production factory
        /// returns an <c>RpcClient</c>; tests supply a fake so the session can be exercised without a socket.</param>
        public DelugeSession(Func<IPEndPoint, IRpcClient> clientFactory)
        {
            ArgumentNullException.ThrowIfNull(clientFactory);
            _ClientFactory = clientFactory;
        }

        /// <summary>Connects to the daemon at the specified <paramref name="endpoint" /> and runs the bootstrap handshake.</summary>
        /// <param name="endpoint">The resolved daemon endpoint. Resolving a host name to an endpoint is the caller's job,
        /// mirroring how <see cref="IRpcClient" /> itself takes an <see cref="IPEndPoint" />.</param>
        /// <param name="username">The account user name.</param>
        /// <param name="password">The account password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">Thrown when the session is not <see cref="SessionState.Disconnected" />
        /// or <see cref="SessionState.Faulted" />.</exception>
        public async Task ConnectAsync(IPEndPoint endpoint, string username, string password, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(endpoint);
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(password);
            if (State is not (SessionState.Disconnected or SessionState.Faulted))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.DelugeSession_CannotConnectInState, State));

            IRpcClient client = _ClientFactory(endpoint);
            _Client = client;
            try
            {
                SetState(SessionState.Connecting);
                await client.StartAsync(cancellationToken)
                    .ConfigureAwait(false);

                InfoRequest.Response info = await client.SendRequestAsync(new InfoRequest(), cancellationToken)
                    .ConfigureAwait(false);

                SetState(SessionState.Authenticating);
                LoginRequest.Response login = await client.SendRequestAsync(new LoginRequest(username, password), cancellationToken)
                    .ConfigureAwait(false);
                AuthenticationLevel = login.AuthenticationLevel;

                await client.SendRequestAsync(new SetEventInterestRequest(), cancellationToken)
                    .ConfigureAwait(false);

                GetMethodList.Response methodList = await client.SendRequestAsync(new GetMethodList(), cancellationToken)
                    .ConfigureAwait(false);
                IReadOnlyCollection<string> methods = methodList.Methods;

                string libtorrentVersion = string.Empty;
                if (methods.Contains(LibtorrentVersionMethod))
                {
                    GetLibTorrentVersionRequest.Response libtorrent = await client.SendRequestAsync(new GetLibTorrentVersionRequest(), cancellationToken)
                        .ConfigureAwait(false);
                    libtorrentVersion = libtorrent.Version;
                }

                Capabilities = new ServerCapabilities(info.Version, libtorrentVersion, methods);
                _Monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
                _Monitor.Start();
                SetState(SessionState.Connected);
            } catch
            {
                await FaultAsync(client)
                    .ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>Pauses the specified torrents.</summary>
        /// <param name="torrentIds">The identifiers of the torrents to pause.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task PauseAsync(IEnumerable<string> torrentIds, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(torrentIds);
            return SendCommandAsync(PauseTorrentsRequest.MethodName, new PauseTorrentsRequest(torrentIds), cancellationToken);
        }

        /// <summary>Resumes the specified torrents.</summary>
        /// <param name="torrentIds">The identifiers of the torrents to resume.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task ResumeAsync(IEnumerable<string> torrentIds, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(torrentIds);
            return SendCommandAsync(ResumeTorrentsRequest.MethodName, new ResumeTorrentsRequest(torrentIds), cancellationToken);
        }

        /// <summary>Removes a torrent from the session.</summary>
        /// <param name="torrentId">The identifier of the torrent to remove.</param>
        /// <param name="removeData">Whether to also remove the downloaded data from disk.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>true</c> if the torrent was removed.</returns>
        public async Task<bool> RemoveAsync(string torrentId, bool removeData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(torrentId);
            RemoveTorrentRequest.Response response = await SendCommandAsync(RemoveTorrentRequest.MethodName, new RemoveTorrentRequest(torrentId, removeData), cancellationToken)
                .ConfigureAwait(false);
            return response.Success;
        }

        /// <summary>Removes several torrents from the session in a single request.</summary>
        /// <param name="torrentIds">The identifiers of the torrents to remove.</param>
        /// <param name="removeData">Whether to also remove the downloaded data from disk.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The torrents that could not be removed; empty when every torrent was removed.</returns>
        public async Task<IReadOnlyList<RemoveTorrentError>> RemoveAsync(IEnumerable<string> torrentIds, bool removeData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(torrentIds);
            RemoveTorrentsRequest.Response response = await SendCommandAsync(RemoveTorrentsRequest.MethodName, new RemoveTorrentsRequest(torrentIds, removeData), cancellationToken)
                .ConfigureAwait(false);
            return response.Errors.ToArray();
        }

        /// <summary>Adds a torrent from a magnet link.</summary>
        /// <param name="magnetUri">The magnet link.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The identifier of the added torrent, or <c>null</c>.</returns>
        public async Task<string?> AddMagnetAsync(string magnetUri, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(magnetUri);
            AddTorrentMagnetRequest.Response response = await SendCommandAsync(AddTorrentMagnetRequest.MethodName, new AddTorrentMagnetRequest(magnetUri), cancellationToken)
                .ConfigureAwait(false);
            return response.TorrentId;
        }

        /// <summary>Adds a torrent from the contents of a torrent file.</summary>
        /// <param name="fileName">The name of the torrent file.</param>
        /// <param name="fileContent">The raw contents of the torrent file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The identifier of the added torrent, or <c>null</c>.</returns>
        public async Task<string?> AddTorrentFileAsync(string fileName, byte[] fileContent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(fileName);
            ArgumentNullException.ThrowIfNull(fileContent);
            AddTorrentFileRequest.Response response = await SendCommandAsync(AddTorrentFileRequest.MethodName, new AddTorrentFileRequest(fileName, fileContent), cancellationToken)
                .ConfigureAwait(false);
            return response.TorrentId;
        }

        /// <summary>Gets the sidebar filter tree: the filter categories the daemon exposes, each with its values and torrent counts.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The filters keyed by category (for example <c>state</c>, <c>tracker_host</c>, <c>owner</c>).</returns>
        public async Task<IReadOnlyDictionary<string, IReadOnlyList<Filter>>> GetFilterTreeAsync(CancellationToken cancellationToken = default)
        {
            GetFilterTreeRequest.Response response = await SendCommandAsync(GetFilterTreeRequest.MethodName, new GetFilterTreeRequest(showZeroHits: true, hiddenCategories: null), cancellationToken)
                .ConfigureAwait(false);
            return response.Filters.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<Filter>)pair.Value.ToArray(),
                StringComparer.Ordinal);
        }

        /// <summary>Narrows the monitored torrents to those matching <paramref name="filter" />.</summary>
        /// <param name="filter">The filter to narrow by, keyed by filter category. An empty filter clears the narrowing.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task SetTorrentFilterAsync(IReadOnlyDictionary<string, string> filter, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            TorrentMonitor? monitor = _Monitor;
            return monitor is null ? Task.CompletedTask : monitor.SetFilterAsync(filter, cancellationToken);
        }

        /// <summary>Gets a value indicating whether the connected daemon supports the specified <paramref name="method" />.</summary>
        /// <param name="method">The fully-qualified RPC method name (for example <c>core.pause_torrents</c>).</param>
        /// <returns><c>true</c> when the daemon advertises the method; <c>false</c> when it does not or while disconnected.</returns>
        public bool Supports(string method)
        {
            return Capabilities?.HasMethod(method) ?? false;
        }

        private async Task<TResponse> SendCommandAsync<TResponse>(string method, RpcRequest<TResponse> request, CancellationToken cancellationToken)
            where TResponse:
                RpcResponse
        {
            IRpcClient? client = _Client;
            if (State != SessionState.Connected || client is null)
                throw new InvalidOperationException(SR.DelugeSession_CommandRequiresConnection);
            if (!Supports(method))
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, SR.DelugeSession_MethodNotSupported, method));

            return await client.SendRequestAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>Disconnects from the daemon and returns the session to <see cref="SessionState.Disconnected" />.</summary>
        public async Task DisconnectAsync()
        {
            await StopMonitorAsync()
                .ConfigureAwait(false);
            IRpcClient? client = _Client;
            if (client != null)
                await client.StopAsync()
                    .ConfigureAwait(false);
            _Client = null;
            Reset();
            SetState(SessionState.Disconnected);
        }

        /// <summary>Stops the underlying client and releases the session.</summary>
        /// <returns>A task that completes when the session has been released.</returns>
        public async ValueTask DisposeAsync()
        {
            await StopMonitorAsync()
                .ConfigureAwait(false);
            IRpcClient? client = _Client;
            if (client != null)
                await client.StopAsync()
                    .ConfigureAwait(false);
            _Client = null;
        }

        private async Task FaultAsync(IRpcClient client)
        {
            await StopMonitorAsync()
                .ConfigureAwait(false);
            Reset();
            SetState(SessionState.Faulted);
            if (ReferenceEquals(_Client, client))
                _Client = null;
            await client.StopAsync()
                .ConfigureAwait(false);
        }

        private async Task StopMonitorAsync()
        {
            TorrentMonitor? monitor = _Monitor;
            _Monitor = null;
            if (monitor != null)
                await monitor.DisposeAsync()
                    .ConfigureAwait(false);
        }

        private void Reset()
        {
            Capabilities = null;
            AuthenticationLevel = 0;
        }

        private void SetState(SessionState state)
        {
            if (State == state)
                return;
            State = state;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Gets the current lifecycle state of the session.</summary>
        public SessionState State { get; private set; } = SessionState.Disconnected;

        /// <summary>Gets the capabilities of the connected daemon, or <c>null</c> while not connected.</summary>
        public ServerCapabilities? Capabilities { get; private set; }

        /// <summary>Gets the authentication level granted by the daemon on the last successful login.</summary>
        public int AuthenticationLevel { get; private set; }

        /// <summary>Gets the torrent monitor for the current connection, or <c>null</c> while not connected.</summary>
        public TorrentMonitor? Torrents => _Monitor;

        /// <summary>Gets the endpoint of the current connection, or <c>null</c> while not connected.</summary>
        public IPEndPoint? Host => _Client?.Host;

        /// <summary>Raised after every transition of <see cref="State" />.</summary>
        public event EventHandler? StateChanged;

        private const string LibtorrentVersionMethod = "core.get_libtorrent_version";

        private readonly Func<IPEndPoint, IRpcClient> _ClientFactory;
        private IRpcClient? _Client;
        private TorrentMonitor? _Monitor;
    }
}
