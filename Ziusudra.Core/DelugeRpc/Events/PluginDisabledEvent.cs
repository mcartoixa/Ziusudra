using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a plugin is disabled in the Core.</summary>
    internal class PluginDisabledEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="PluginDisabledEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal PluginDisabledEvent(ICollection values):
            base(values)
        { }

        /// <summary>The name of the plugin.</summary>
        public string PluginName => Data[0]?.ToString() ?? string.Empty;
    }
}
