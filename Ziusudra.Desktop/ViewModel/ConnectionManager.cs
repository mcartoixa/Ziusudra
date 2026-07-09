using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ziusudra.Client;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>Presents the connection manager: the saved host list plus connect/authenticate against the daemon.</summary>
    /// <remarks>A thin wrapper over <see cref="Client.ConnectionManager" />; the logic lives there so it stays unit-testable.
    /// Session state changes arrive on a background thread and are marshalled onto the UI thread via <see cref="IUIDispatcher" />.</remarks>
    public sealed partial class ConnectionManager:
        ObservableObject
    {

        /// <summary>Create a new instance of the <see cref="ConnectionManager" /> type.</summary>
        /// <param name="connections">The connection manager.</param>
        /// <param name="dispatcher">The UI dispatcher.</param>
        public ConnectionManager(Client.ConnectionManager connections, IUIDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(connections);
            ArgumentNullException.ThrowIfNull(dispatcher);

            _Connections = connections;
            _Dispatcher = dispatcher;
            _Connections.StateChanged += OnConnectionsStateChanged;
            _Connections.HostStatusChanged += OnHostStatusChanged;
        }

        /// <summary>Loads the saved host list and refreshes the status.</summary>
        public async Task InitializeAsync()
        {
            await _Connections.LoadAsync();
            Hosts.Clear();
            foreach (HostEntry host in _Connections.Hosts)
                Hosts.Add(new HostRow(host));
            UpdateState();
        }

        /// <summary>Gets the saved host list.</summary>
        public ObservableCollection<HostRow> Hosts { get; } = new();

        /// <summary>The selected host.</summary>
        [ObservableProperty]
        private HostRow? _SelectedHost;

        /// <summary>The password used to connect. It is not persisted.</summary>
        [ObservableProperty]
        private string _Password = string.Empty;

        /// <summary>The current status message.</summary>
        [ObservableProperty]
        private string _StatusText = string.Empty;

        /// <summary>Whether the session is connected.</summary>
        [ObservableProperty]
        private bool _IsConnected;

        /// <summary>Adds a new host from the entered values and persists it. The view gathers and parses the input.</summary>
        /// <param name="name">The display name.</param>
        /// <param name="host">The host name or address.</param>
        /// <param name="port">The port.</param>
        /// <param name="username">The account user name.</param>
        public async Task AddHostAsync(string name, string host, string port, string username)
        {
            if (string.IsNullOrWhiteSpace(host))
                return;

            var entry = new HostEntry {
                Name = string.IsNullOrWhiteSpace(name) ? host : name,
                Host = host,
                Port = ParsePort(port),
                Username = username
            };
            await _Connections.AddAsync(entry);
            Hosts.Add(new HostRow(entry));
        }

        [RelayCommand]
        private async Task RemoveHostAsync()
        {
            if (SelectedHost is not HostRow row)
                return;

            await _Connections.RemoveAsync(row.Entry);
            Hosts.Remove(row);
        }

        /// <summary>Applies edited values to the selected host and persists them. The view gathers and parses the input.</summary>
        /// <param name="row">The host row to update.</param>
        /// <param name="name">The new display name.</param>
        /// <param name="host">The new host name or address.</param>
        /// <param name="port">The new port.</param>
        /// <param name="username">The new account user name.</param>
        public async Task UpdateHostAsync(HostRow row, string name, string host, string port, string username)
        {
            ArgumentNullException.ThrowIfNull(row);
            if (string.IsNullOrWhiteSpace(host))
                return;

            row.Entry.Name = string.IsNullOrWhiteSpace(name) ? host : name;
            row.Entry.Host = host;
            row.Entry.Port = ParsePort(port);
            row.Entry.Username = username;
            await _Connections.UpdateAsync(row.Entry);
            row.Refresh();
        }

        private static int ParsePort(string port)
        {
            return int.TryParse(port, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : HostEntry.DefaultPort;
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            if (SelectedHost is not HostRow row)
                return;

            try
            {
                StatusText = "Connecting…";
                await _Connections.ConnectAsync(row.Entry, Password);
            } catch (Exception ex)
            {
                StatusText = ex.Message;
            }
        }

        [RelayCommand]
        private Task DisconnectAsync()
        {
            return _Connections.DisconnectAsync();
        }

        /// <summary>Re-probes the reachability and daemon version of every saved host.</summary>
        [RelayCommand]
        private Task RefreshStatusAsync()
        {
            return _Connections.RefreshStatusAsync();
        }

        private void OnConnectionsStateChanged(object? sender, EventArgs e)
        {
            _Dispatcher.Post(UpdateState);
        }

        private void OnHostStatusChanged(object? sender, HostStatusChangedEventArgs e)
        {
            _Dispatcher.Post(() => {
                HostRow? row = Hosts.FirstOrDefault(h => h.Entry.Id == e.HostId);
                row?.SetStatus(e.Status, e.Version);
            });
        }

        private void UpdateState()
        {
            SessionState state = _Connections.State;
            IsConnected = state == SessionState.Connected;
            StatusText = state switch {
                SessionState.Connected => $"Connected to Deluge {_Connections.Capabilities?.DaemonVersion}.",
                SessionState.Connecting => "Connecting…",
                SessionState.Authenticating => "Authenticating…",
                SessionState.Faulted => "Connection failed.",
                _ => "Disconnected."
            };

            // A terminal transition re-probes the hosts: the active host flips to Connected, and a former host is re-checked.
            if (state is SessionState.Disconnected or SessionState.Faulted)
                _ = _Connections.RefreshStatusAsync();
        }

        private readonly Client.ConnectionManager _Connections;
        private readonly IUIDispatcher _Dispatcher;
    }
}
