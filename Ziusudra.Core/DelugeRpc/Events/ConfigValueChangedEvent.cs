using System.Collections;

namespace Ziusudra.DelugeRpc.Events
{

    /// <summary>Emitted when a config value changes in the Core.</summary>
    internal class ConfigValueChangedEvent:
        RpcEvent
    {

        /// <summary>Create a new instance of the <see cref="ConfigValueChangedEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal ConfigValueChangedEvent(ICollection values):
            base(values)
        { }

        /// <summary>The key that changed.</summary>
        public string Key => Data[0]?.ToString() ?? string.Empty;
        /// <summary>The new value.</summary>
        public string Value => Data[1]?.ToString() ?? string.Empty;
    }
}
