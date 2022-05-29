using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when the external ip address is received from libtorrent.</summary>
    internal class ExternalIPEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="ExternalIPEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal ExternalIPEvent(ICollection values):
            base(values)
        { }

        /// <summary>The IP address.</summary>
        public string ExternalIP => Data[0]?.ToString() ?? string.Empty;
    }
}
