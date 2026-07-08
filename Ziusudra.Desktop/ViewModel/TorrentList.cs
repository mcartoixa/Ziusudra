using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ziusudra.Client;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>Projects the session's torrent monitor into a bound collection, marshalling onto the UI thread.</summary>
    /// <remarks>The monitor is thread-agnostic and raises its notifications on background threads; every model
    /// mutation here is posted through <see cref="IUIDispatcher" /> so the <see cref="ObservableCollection{T}" /> and
    /// its rows are only ever touched on the UI thread.</remarks>
    public sealed partial class TorrentList:
        ObservableObject
    {

        /// <summary>Create a new instance of the <see cref="TorrentList" /> type.</summary>
        /// <param name="session">The session whose monitor is projected.</param>
        /// <param name="dispatcher">The UI dispatcher.</param>
        public TorrentList(DelugeSession session, IUIDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(dispatcher);

            _Session = session;
            _Dispatcher = dispatcher;
            _Session.StateChanged += OnSessionStateChanged;
        }

        /// <summary>Gets the bound torrent rows.</summary>
        public ObservableCollection<TorrentRow> Torrents { get; } = new();

        /// <summary>The row whose fields the details tabs present: the single selection, empty when zero or many rows are selected.</summary>
        [ObservableProperty]
        private TorrentRow? _SelectedTorrent;

        /// <summary>Gets the rows currently selected in the list; the commands act over this whole set.</summary>
        public IReadOnlyList<TorrentRow> Selection => _Selection;

        /// <summary>Records the current list selection. The view owns extended selection and pushes it here.</summary>
        /// <param name="selected">The rows now selected.</param>
        public void SetSelection(IReadOnlyList<TorrentRow> selected)
        {
            ArgumentNullException.ThrowIfNull(selected);
            _Selection = selected;
            SelectedTorrent = selected.Count == 1 ? selected[0] : null;
            PauseCommand.NotifyCanExecuteChanged();
            ResumeCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanRemove));
        }

        /// <summary>The message from the last command, shown to the user; empty when it succeeded.</summary>
        [ObservableProperty]
        private string _CommandStatus = string.Empty;

        /// <summary>The magnet link to add.</summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddMagnetCommand))]
        private string _NewMagnet = string.Empty;

        /// <summary>Gets a value indicating whether a torrent file can be added to the connected daemon.</summary>
        public bool CanAddFile => _Session.State == SessionState.Connected && _Session.Supports(AddTorrentFileRequest.MethodName);

        /// <summary>Adds the torrent at <see cref="NewMagnet" /> from its magnet link.</summary>
        [RelayCommand(CanExecute = nameof(CanAddMagnet))]
        private Task AddMagnetAsync()
        {
            string magnet = NewMagnet;
            if (string.IsNullOrWhiteSpace(magnet))
                return Task.CompletedTask;

            return RunAsync(async () => {
                await _Session.AddMagnetAsync(magnet);
                NewMagnet = string.Empty;
            });
        }

        private bool CanAddMagnet()
        {
            return !string.IsNullOrWhiteSpace(NewMagnet) && _Session.State == SessionState.Connected && _Session.Supports(AddTorrentMagnetRequest.MethodName);
        }

        /// <summary>Adds a torrent from the contents of a picked torrent file. The picking itself is the view's concern.</summary>
        /// <param name="fileName">The name of the torrent file.</param>
        /// <param name="content">The raw contents of the torrent file.</param>
        public Task AddFileAsync(string fileName, byte[] content)
        {
            return RunAsync(() => _Session.AddTorrentFileAsync(fileName, content));
        }

        /// <summary>Pauses the selected torrents.</summary>
        [RelayCommand(CanExecute = nameof(CanPause))]
        private Task PauseAsync()
        {
            string[] ids = SelectedIds();
            return ids.Length == 0 ? Task.CompletedTask : RunAsync(() => _Session.PauseAsync(ids));
        }

        private bool CanPause() => CanCommand(PauseTorrentsRequest.MethodName);

        /// <summary>Resumes the selected torrents.</summary>
        [RelayCommand(CanExecute = nameof(CanResume))]
        private Task ResumeAsync()
        {
            string[] ids = SelectedIds();
            return ids.Length == 0 ? Task.CompletedTask : RunAsync(() => _Session.ResumeAsync(ids));
        }

        private bool CanResume() => CanCommand(ResumeTorrentsRequest.MethodName);

        /// <summary>Gets a value indicating whether the selected torrents can be removed from the connected daemon.</summary>
        public bool CanRemove => CanCommand(RemoveTorrentsRequest.MethodName);

        /// <summary>Removes the selected torrents. Whether to also delete their data is the caller's choice, gathered by the view.</summary>
        /// <param name="removeData">Whether to delete the torrents' downloaded data from disk.</param>
        public Task RemoveSelectedAsync(bool removeData)
        {
            string[] ids = SelectedIds();
            if (ids.Length == 0)
                return Task.CompletedTask;

            return RunAsync(async () => {
                IReadOnlyList<RemoveTorrentError> errors = await _Session.RemoveAsync(ids, removeData);
                if (errors.Count > 0)
                    CommandStatus = $"Failed to remove {errors.Count} of {ids.Length} torrents.";
            });
        }

        private string[] SelectedIds() => _Selection.Select(row => row.Hash).ToArray();

        private bool CanCommand(string method)
        {
            return _Selection.Count > 0 && _Session.State == SessionState.Connected && _Session.Supports(method);
        }

        private async Task RunAsync(Func<Task> command)
        {
            try
            {
                CommandStatus = string.Empty;
                await command();
            } catch (Exception ex)
            {
                CommandStatus = ex.Message;
            }
        }

        private void OnSessionStateChanged(object? sender, EventArgs e)
        {
            _Dispatcher.Post(SyncToSession);
        }

        private void SyncToSession()
        {
            if (_Session.State == SessionState.Connected)
                Attach(_Session.Torrents);
            else
                Detach();

            PauseCommand.NotifyCanExecuteChanged();
            ResumeCommand.NotifyCanExecuteChanged();
            AddMagnetCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanRemove));
            OnPropertyChanged(nameof(CanAddFile));
        }

        private void Attach(TorrentMonitor? monitor)
        {
            if (monitor == null || ReferenceEquals(monitor, _Attached))
                return;

            Detach();
            _Attached = monitor;
            monitor.TorrentAdded += OnTorrentAdded;
            monitor.TorrentUpdated += OnTorrentUpdated;
            monitor.TorrentRemoved += OnTorrentRemoved;

            // Seed from the current snapshot; the monitor's own poll fills and keeps the list current thereafter.
            foreach (Torrent torrent in monitor.Torrents)
                AddOrUpdateRow(torrent);
        }

        private void Detach()
        {
            if (_Attached != null)
            {
                _Attached.TorrentAdded -= OnTorrentAdded;
                _Attached.TorrentUpdated -= OnTorrentUpdated;
                _Attached.TorrentRemoved -= OnTorrentRemoved;
                _Attached = null;
            }
            _Rows.Clear();
            Torrents.Clear();
        }

        private void OnTorrentAdded(object? sender, TorrentChangedEventArgs e)
        {
            Torrent torrent = e.Torrent;
            _Dispatcher.Post(() => AddOrUpdateRow(torrent));
        }

        private void OnTorrentUpdated(object? sender, TorrentChangedEventArgs e)
        {
            Torrent torrent = e.Torrent;
            _Dispatcher.Post(() => AddOrUpdateRow(torrent));
        }

        private void OnTorrentRemoved(object? sender, TorrentChangedEventArgs e)
        {
            string hash = e.Torrent.Hash;
            _Dispatcher.Post(() => RemoveRow(hash));
        }

        private void AddOrUpdateRow(Torrent torrent)
        {
            if (_Rows.TryGetValue(torrent.Hash, out TorrentRow? row))
            {
                row.Update(torrent);
                return;
            }
            var added = new TorrentRow(torrent);
            _Rows[torrent.Hash] = added;
            Torrents.Add(added);
        }

        private void RemoveRow(string hash)
        {
            if (_Rows.Remove(hash, out TorrentRow? row))
                Torrents.Remove(row);
        }

        private readonly DelugeSession _Session;
        private readonly IUIDispatcher _Dispatcher;
        private readonly Dictionary<string, TorrentRow> _Rows = new(StringComparer.Ordinal);
        private IReadOnlyList<TorrentRow> _Selection = Array.Empty<TorrentRow>();
        private TorrentMonitor? _Attached;
    }
}
