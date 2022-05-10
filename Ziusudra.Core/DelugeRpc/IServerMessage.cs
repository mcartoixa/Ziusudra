namespace Ziusudra.DelugeRpc

{

    /// <summary>Represents a message from a Deluge server.</summary>
    public interface IServerMessage:
        IMessage
    {

        /// <summary>Gets the type of message.</summary>
        RpcMessageType MessageType { get; }
    }
}
