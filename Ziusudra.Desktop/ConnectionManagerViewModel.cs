using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ziusudra.Client;

namespace Ziusudra.Desktop
{

    /// <summary>Presents the connection manager: the saved host list plus connect/authenticate against the daemon.</summary>
    /// <remarks>A thin wrapper over <see cref="ConnectionManager" />; the logic lives there so it stays unit-testable.
    /// Session state changes arrive on a background thread and are marshalled onto the UI thread via <see cref="IUIDispatcher" />.</remarks>
    public sealed partial class ConnectionManagerViewModel:
        ObservableObject
    {

        /// <summary>Create a new instance of the <see cref="ConnectionManagerViewModel" /> type.</summary>
        /// <param name="connections">The connection manager.</param>
        /// <param name="dispatcher">The UI dispatcher.</param>
        public ConnectionManagerViewModel(ConnectionManager connections, IUIDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(connections);
            ArgumentNullException.ThrowIfNull(dispatcher);

            _Connections = connections;
            _Dispatcher = dispatcher;
            _Connections.StateChanged += OnConnectionsStateChanged;
            NewPort = HostEntry.DefaultPort.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Loads the saved host list and refreshes the status.</summary>
        public async Task InitializeAsync()
        {
            await _Connections.LoadAsync();
            Hosts.Clear();
            foreach (HostEntry host in _Connections.Hosts)
                Hosts.Add(host);
            UpdateState();
        }

        /// <summary>Gets the saved host list.</summary>
        public ObservableCollection<HostEntry> Hosts { get; } = new();

        /// <summary>The selected host.</summary>
        [ObservableProperty]
        private HostEntry? _SelectedHost;

        /// <summary>The name for a new host entry.</summary>
        [ObservableProperty]
        private string _NewName = string.Empty;

        /// <summary>The host name for a new host entry.</summary>
        [ObservableProperty]
        private string _NewHost = string.Empty;

        /// <summary>The port for a new host entry.</summary>
        [ObservableProperty]
        private string _NewPort = string.Empty;

        /// <summary>The user name for a new host entry.</summary>
        [ObservableProperty]
        private string _NewUsername = string.Empty;

        /// <summary>The password used to connect. It is not persisted.</summary>
        [ObservableProperty]
        private string _Password = string.Empty;

        /// <summary>The current status message.</summary>
        [ObservableProperty]
        private string _StatusText = string.Empty;

        /// <summary>Whether the session is connected.</summary>
        [ObservableProperty]
        private bool _IsConnected;

        [RelayCommand]
        private async Task AddHostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewHost))
                return;

            int port = int.TryParse(NewPort, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ? parsed : HostEntry.DefaultPort;
            var entry = new HostEntry {
                Name = string.IsNullOrWhiteSpace(NewName) ? NewHost : NewName,
                Host = NewHost,
                Port = port,
                Username = NewUsername
            };
            await _Connections.AddAsync(entry);
            Hosts.Add(entry);

            NewName = string.Empty;
            NewHost = string.Empty;
            NewUsername = string.Empty;
            NewPort = HostEntry.DefaultPort.ToString(CultureInfo.InvariantCulture);
        }

        [RelayCommand]
        private async Task RemoveHostAsync()
        {
            if (SelectedHost is not HostEntry entry)
                return;

            await _Connections.RemoveAsync(entry);
            Hosts.Remove(entry);
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            if (SelectedHost is not HostEntry entry)
                return;

            try
            {
                StatusText = "Connecting…";
                await _Connections.ConnectAsync(entry, Password);
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

        private void OnConnectionsStateChanged(object? sender, EventArgs e)
        {
            _Dispatcher.Post(UpdateState);
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
        }

        private readonly ConnectionManager _Connections;
        private readonly IUIDispatcher _Dispatcher;
    }
}
