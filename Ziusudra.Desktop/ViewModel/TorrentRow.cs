using CommunityToolkit.Mvvm.ComponentModel;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>Presents one torrent row in the list. Updated in place so unchanged fields raise no notification.</summary>
    public sealed partial class TorrentRow:
        ObservableObject
    {

        /// <summary>Create a new instance of the <see cref="TorrentRow" /> type.</summary>
        /// <param name="torrent">The torrent to present.</param>
        public TorrentRow(Torrent torrent)
        {
            ArgumentNullException.ThrowIfNull(torrent);
            Hash = torrent.Hash;
            Update(torrent);
        }

        /// <summary>Gets the hash identifying the torrent.</summary>
        public string Hash { get; }

        /// <summary>Refreshes the row from the latest <paramref name="torrent" /> state.</summary>
        /// <param name="torrent">The latest torrent state.</param>
        public void Update(Torrent torrent)
        {
            ArgumentNullException.ThrowIfNull(torrent);
            Name = torrent.Name ?? string.Empty;
            State = torrent.State?.ToString() ?? string.Empty;
            Progress = torrent.Progress ?? 0;
            DownloadRate = torrent.DownloadPayloadRate ?? 0;
            UploadRate = torrent.UploadPayloadRate ?? 0;
        }

        /// <summary>The torrent name.</summary>
        [ObservableProperty]
        private string _Name = string.Empty;

        /// <summary>The torrent state.</summary>
        [ObservableProperty]
        private string _State = string.Empty;

        /// <summary>The download progress, from 0 to 1.</summary>
        [ObservableProperty]
        private double _Progress;

        /// <summary>The download rate, in bytes per second.</summary>
        [ObservableProperty]
        private int _DownloadRate;

        /// <summary>The upload rate, in bytes per second.</summary>
        [ObservableProperty]
        private int _UploadRate;
    }
}
