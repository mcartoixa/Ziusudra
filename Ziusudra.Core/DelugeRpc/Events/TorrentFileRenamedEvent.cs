using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a file within a torrent has been renamed.</summary>
    internal class TorrentFileRenamedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentFileRenamedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentFileRenamedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The index of the file.</summary>
        public int FileIndex => Convert.ToInt32(Data[1]);
        /// <summary>The new file name.</summary>
        public string FileName => Data[2]?.ToString() ?? string.Empty;
    }
}
