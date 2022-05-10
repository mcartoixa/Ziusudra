namespace Ziusudra.DelugeRpc
{

    /// <summary>The types of messages sent by the server.</summary>
    public enum RpcMessageType
    {
        /// <summary>A response to a request sent by the client.</summary>
        RPC_RESPONSE = 1,
        /// <summary>An error that occurrend on the server wg=hile processing a request.</summary>
        RPC_ERROR = 2,
        /// <summary>A server event.</summary>
        RPC_EVENT = 3
    }
}