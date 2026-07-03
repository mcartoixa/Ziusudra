using System.Net;

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
        public ConnectionManager(DelugeSession session, IHostStore store, IHostResolver resolver)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(resolver);

            _Session = session;
            _Store = store;
            _Resolver = resolver;
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
        }

        /// <summary>Disconnects the session.</summary>
        public Task DisconnectAsync()
        {
            return _Session.DisconnectAsync();
        }

        private void OnSessionStateChanged(object? sender, EventArgs e)
        {
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

        private readonly List<HostEntry> _Hosts = new();
        private readonly IHostResolver _Resolver;
        private readonly DelugeSession _Session;
        private readonly IHostStore _Store;
    }
}
