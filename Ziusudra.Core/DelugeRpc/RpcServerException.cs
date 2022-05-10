using System.Collections;

namespace Ziusudra.DelugeRpc
{

    /// <summary>An error that occurred on the server while processing a request.</summary>
    public sealed class RpcServerException:
        RpcException,
        IServerReply
    {

        internal RpcServerException(ICollection values):
            base(GetMessageFromValues(values))
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

        private static string GetMessageFromValues(ICollection values)
        {
            return values.Cast<object?>().ElementAtOrDefault(3) as string ?? string.Empty;
        }

        int IExchangeMessage.Id => Convert.ToInt32(Values[1]);

        RpcMessageType IServerMessage.MessageType => (RpcMessageType)Convert.ToInt32(Values[0]);

        internal IList Values { get; }
    }
}
