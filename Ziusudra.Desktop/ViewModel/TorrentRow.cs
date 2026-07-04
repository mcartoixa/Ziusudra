using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>Presents one torrent for the list and the details tabs. Updated in place so unchanged fields raise no notification.</summary>
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
            ProgressText = HumanReadable.Percent(torrent.Progress);
            DownloadRateText = HumanReadable.Rate(torrent.DownloadPayloadRate);
            UploadRateText = HumanReadable.Rate(torrent.UploadPayloadRate);
            EtaText = HumanReadable.Duration(torrent.ExpectedTimeOfArrival);
            SizeText = HumanReadable.Bytes(torrent.TotalWanted);
            QueueText = torrent.Queue?.ToString(CultureInfo.CurrentCulture) ?? Dash;
        }

        /// <summary>The torrent name.</summary>
        [ObservableProperty]
        private string _Name = string.Empty;

        /// <summary>The torrent state.</summary>
        [ObservableProperty]
        private string _State = string.Empty;

        /// <summary>The download progress, from 0 to 1, for a progress bar.</summary>
        [ObservableProperty]
        private double _Progress;

        /// <summary>The download progress as a percentage.</summary>
        [ObservableProperty]
        private string _ProgressText = string.Empty;

        /// <summary>The download rate.</summary>
        [ObservableProperty]
        private string _DownloadRateText = string.Empty;

        /// <summary>The upload rate.</summary>
        [ObservableProperty]
        private string _UploadRateText = string.Empty;

        /// <summary>The estimated time until the download completes.</summary>
        [ObservableProperty]
        private string _EtaText = string.Empty;

        /// <summary>The wanted size of the torrent.</summary>
        [ObservableProperty]
        private string _SizeText = string.Empty;

        /// <summary>The position of the torrent in the queue.</summary>
        [ObservableProperty]
        private string _QueueText = string.Empty;

        private const string Dash = "—";
    }
}
