namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Reports a torrent that could not be removed by a <see cref="RemoveTorrentsRequest" />.</summary>
    public class RemoveTorrentError
    {

        internal RemoveTorrentError(string torrentId, string message)
        {
            TorrentId = torrentId;
            Message = message;
        }

        /// <summary>Gets the identifier of the torrent that could not be removed.</summary>
        public string TorrentId { get; }

        /// <summary>Gets the message describing why the torrent could not be removed.</summary>
        public string Message { get; }
    }
}
