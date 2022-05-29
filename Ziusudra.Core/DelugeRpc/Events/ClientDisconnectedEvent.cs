using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a client disconnects.</summary>
    internal class ClientDisconnectedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="ClientDisconnectedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal ClientDisconnectedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The identifier of the session.</summary>
        public string SessionId => Data[0]?.ToString() ?? string.Empty;
    }
}
