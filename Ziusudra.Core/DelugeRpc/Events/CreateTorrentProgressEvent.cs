using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when creating a torrent file remotely.</summary>
    internal class CreateTorrentProgressEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="CreateTorrentProgressEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal CreateTorrentProgressEvent(ICollection values):
            base(values)
        { }

        /// <summary />
        public int PieceCount => Convert.ToInt32(Data[0]);
        /// <summary />
        public int NumPieces => Convert.ToInt32(Data[1]);
    }
}
