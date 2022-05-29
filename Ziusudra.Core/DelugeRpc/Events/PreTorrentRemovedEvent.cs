using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a torrent is about to be removed from the session.</summary>
    internal class PreTorrentRemovedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentAddedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal PreTorrentRemovedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
    }
}
