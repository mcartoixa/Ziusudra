using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when the session has been paused.</summary>
    internal class SessionPausedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="SessionPausedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal SessionPausedEvent(ICollection values):
            base(values)
        { }
    }
}
