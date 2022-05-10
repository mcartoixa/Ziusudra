using System.Collections;

namespace Ziusudra.DelugeRpc
{

    /// <summary>An event sent by the server.</summary>
    public class RpcEvent:
        IServerMessage
    {

        /// <summary>Create a new instance of the <see cref="RpcEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        internal protected RpcEvent(ICollection values)
        {
            if (values is IList l)
                Values = l;
            else
                Values = new ArrayList(values);
        }

        ICollection IMessage.ToValueCollection()
        {
            return Values;
        }

        RpcMessageType IServerMessage.MessageType => (RpcMessageType)Convert.ToInt32(Values[0]);

        /// <summary>Gets the values of the event.</summary>
        internal protected IList Values { get; }
    }
}
