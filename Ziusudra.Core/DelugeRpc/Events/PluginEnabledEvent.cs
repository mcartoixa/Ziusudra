using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a plugin is enabled in the Core.</summary>
    internal class PluginEnabledEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="PluginEnabledEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal PluginEnabledEvent(ICollection values):
            base(values)
        { }

        /// <summary>The name of the plugin.</summary>
        public string PluginName => Data[0]?.ToString() ?? string.Empty;
    }
}
