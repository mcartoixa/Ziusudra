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

        /// <summary>The row selected in the list, whose fields the details tabs present.</summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PauseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ResumeCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        private TorrentRow? _SelectedTorrent;

        /// <summary>The message from the last command, shown to the user; empty when it succeeded.</summary>
        [ObservableProperty]
        private string _CommandStatus = string.Empty;

        /// <summary>Pauses the selected torrent.</summary>
        [RelayCommand(CanExecute = nameof(CanPause))]
        private Task PauseAsync()
        {
            TorrentRow? selected = SelectedTorrent;
            return selected is null ? Task.CompletedTask : RunAsync(() => _Session.PauseAsync(new[] { selected.Hash }));
        }

        private bool CanPause() => CanCommand(PauseTorrentsRequest.MethodName);

        /// <summary>Resumes the selected torrent.</summary>
        [RelayCommand(CanExecute = nameof(CanResume))]
        private Task ResumeAsync()
        {
            TorrentRow? selected = SelectedTorrent;
            return selected is null ? Task.CompletedTask : RunAsync(() => _Session.ResumeAsync(new[] { selected.Hash }));
        }

        private bool CanResume() => CanCommand(ResumeTorrentsRequest.MethodName);

        /// <summary>Removes the selected torrent, leaving its downloaded data on disk.</summary>
        [RelayCommand(CanExecute = nameof(CanRemove))]
        private Task RemoveAsync()
        {
            TorrentRow? selected = SelectedTorrent;
            return selected is null ? Task.CompletedTask : RunAsync(() => _Session.RemoveAsync(selected.Hash, removeData: false));
        }

        private bool CanRemove() => CanCommand(RemoveTorrentRequest.MethodName);

        private bool CanCommand(string method)
        {
            return SelectedTorrent != null && _Session.State == SessionState.Connected && _Session.Supports(method);
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
            RemoveCommand.NotifyCanExecuteChanged();
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
        private TorrentMonitor? _Attached;
    }
}
