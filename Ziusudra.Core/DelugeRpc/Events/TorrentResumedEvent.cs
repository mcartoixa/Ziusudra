using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a torrent resumes from a paused state.</summary>
    internal class TorrentResumedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentResumedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentResumedEvent(ICollection values) :
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
    }
}
