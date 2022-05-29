using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when the queue order has changed.</summary>
    internal class TorrentQueueChangedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="TorrentQueueChangedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal TorrentQueueChangedEvent(ICollection values):
            base(values)
        { }
    }
}
