using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a torrent has been removed from the session.</summary>
    internal class TorrentRemovedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentRemovedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentRemovedEvent(ICollection values) :
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
    }
}
