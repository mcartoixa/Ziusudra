using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when the session has been resumed.</summary>
    internal class SessionResumedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="SessionResumedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal SessionResumedEvent(ICollection values):
            base(values)
        { }
    }
}
