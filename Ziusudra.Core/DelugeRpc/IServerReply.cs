namespace Ziusudra.DelugeRpc
{

    /// <summary>Represents a reply from a server to a client <see cref="IClientRequest" />.</summary>
    public interface IServerReply:
        IExchangeMessage,
        IServerMessage
    {
    }
}
