using System.Collections;
using System.Diagnostics;

namespace Ziusudra.DelugeRpc
{

    /// <summary>An error that occurred on the server while processing a request.</summary>
    public sealed class RpcServerException:
        RpcException,
        IServerReply
    {

        /// <summary>Create a new instance of the <see cref="RpcServerException" /> type.</summary>
        /// <param name="values">The values to create </param>
        public RpcServerException(ICollection values):
            base(GetMessageFromValues(values))
        {
            if (values is IList l)
                Values = l;
            else
                Values = new ArrayList(values);

            Debug.Assert(((IServerMessage)this).MessageType == RpcMessageType.RPC_ERROR);
        }

        ICollection IMessage.ToValueCollection()
        {
            return Values;
        }

        private static string GetMessageFromValues(ICollection values)
        {
            return values.Cast<object?>().ElementAtOrDefault(3) as string ?? string.Empty;
        }

        /// <summary>Get or sets the name of the application or the object that causes the error.</summary>
        public override string? Source => Values[2] as string;

        /// <summary>Get a string representation of the immediate frames on the call stack.</summary>
        public override string? StackTrace => Values[4] as string;

        int IExchangeMessage.Id => Convert.ToInt32(Values[1]);

        RpcMessageType IServerMessage.MessageType => (RpcMessageType)Convert.ToInt32(Values[0]);

        internal IList Values { get; }
    }
}
