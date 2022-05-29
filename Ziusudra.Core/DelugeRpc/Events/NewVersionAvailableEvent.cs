using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a more recent version of Deluge is available.</summary>
    internal class NewVersionAvailableEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="NewVersionAvailableEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal NewVersionAvailableEvent(ICollection values):
            base(values)
        { }

        /// <summary>The new version that is available.</summary>
        public string NewRelease => Data[0]?.ToString() ?? string.Empty;
    }
}
