using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a new torrent is successfully added to the session.</summary>
    internal class TorrentAddedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentAddedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentAddedEvent(ICollection values) :
            base(values)
        { }

        /// <summary>Indicates whether the torrent was loaded from state or if it is a new one.</summary>
        public bool IsFromState => Convert.ToBoolean(Data[1]);
        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
    }
}
