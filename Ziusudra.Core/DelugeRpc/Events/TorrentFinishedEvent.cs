using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a torrent finishes downloading.</summary>
    internal class TorrentFinishedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentFinishedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentFinishedEvent(ICollection values) :
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
    }
}
