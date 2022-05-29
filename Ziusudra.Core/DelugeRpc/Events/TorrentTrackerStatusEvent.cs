using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a torrents tracker status changes.</summary>
    internal class TorrentTrackerStatusEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentTrackerStatusEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentTrackerStatusEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The new status.</summary>
        public string Status => Data[1]?.ToString() ?? string.Empty;
    }
}
