using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a torrent changes state.</summary>
    internal class TorrentStateChangedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentStateChangedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentStateChangedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The new state.</summary>
        public string State => Data[1]?.ToString() ?? string.Empty;
    }
}
