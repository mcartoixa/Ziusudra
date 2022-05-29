using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a session has started.</summary>
    /// <remarks>This typically only happens once when the daemon is initially started.</remarks>
    internal class SessionStartedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="SessionStartedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal SessionStartedEvent(ICollection values):
            base(values)
        { }
    }
}
