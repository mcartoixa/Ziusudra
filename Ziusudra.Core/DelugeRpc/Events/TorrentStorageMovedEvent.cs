using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when the storage location for a torrent has been moved.</summary>
    internal class TorrentStorageMovedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentStorageMovedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentStorageMovedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The new storage location.</summary>
        public string StoragePath => Data[1]?.ToString() ?? string.Empty;
    }
}
