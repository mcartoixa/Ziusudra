using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Client
{

    /// <summary>Carries the torrent a <see cref="TorrentMonitor" /> change notification refers to.</summary>
    public sealed class TorrentChangedEventArgs:
        EventArgs
    {

        /// <summary>Create a new instance of the <see cref="TorrentChangedEventArgs" /> type.</summary>
        /// <param name="torrent">The torrent the notification refers to.</param>
        public TorrentChangedEventArgs(Torrent torrent)
        {
            ArgumentNullException.ThrowIfNull(torrent);
            Torrent = torrent;
        }

        /// <summary>Gets the torrent the notification refers to. For a removal, this is its last known state.</summary>
        public Torrent Torrent { get; }
    }
}
