using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a file completes.</summary>
    internal class TorrentFileCompletedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentFileCompletedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentFileCompletedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The index of the file.</summary>
        public int FileIndex => Convert.ToInt32(Data[1]);
    }
}
