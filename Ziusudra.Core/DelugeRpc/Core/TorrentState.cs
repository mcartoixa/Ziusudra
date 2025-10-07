namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Possible states of a torrent.</summary>
    public enum TorrentState
    {
        /// <summary>The torrent is in the allocating state.</summary>
        Allocating,
        /// <summary>The torrent is in the checking state.</summary>
        Checking,
        /// <summary>The torrent is in the downloading state.</summary>
        Downloading,
        /// <summary>The torrent is in the seeding state.</summary>
        Seeding,
        /// <summary>The torrent is in the paused state.</summary>
        Paused,
        /// <summary>The torrent is in the paused state.</summary>
        Error,
        /// <summary>The torrent is in the queued state.</summary>
        Queued,
        /// <summary>The torrent is in the moving state.</summary>
        Moving
    }
}
