using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a folder within a torrent has been renamed.</summary>
    internal class TorrentFolderRenamedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentFolderRenamedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentFolderRenamedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the torrent.</summary>
        public string TorrentId => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The old folder name.</summary>
        public string OldName => Data[1]?.ToString() ?? string.Empty;
        /// <summary>The new folder name.</summary>
        public string NewName => Data[2]?.ToString() ?? string.Empty;
    }
}
