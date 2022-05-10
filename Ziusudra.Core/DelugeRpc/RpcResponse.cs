using System.Collections;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Base class for server responses.</summary>
    public class RpcResponse:
        IServerReply
    {

        /// <summary>Create a new instance of the <see cref="RpcResponse" /> type.</summary>
        /// <param name="values">The values to create the response with.</param>
        internal protected RpcResponse(ICollection values)
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

        /// <summary>Gets the identifier of the response.</summary>
        /// <remarks>The identifier is used to match a response with a request that had been sent prior.</remarks>
        public int Id => Convert.ToInt32(Values[1]);

        RpcMessageType IServerMessage.MessageType => (RpcMessageType)Convert.ToInt32(Values[0]);

        /// <summary>Gets the values of the response.</summary>
        internal protected IList Values { get; }
    }
}
