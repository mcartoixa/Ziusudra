using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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
        private TorrentRow? _SelectedTorrent;

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
