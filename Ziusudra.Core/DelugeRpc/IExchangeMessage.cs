namespace Ziusudra.DelugeRpc
{

    /// <summary>Represent a <see cref="IMessage" /> that is part of a dialog between a Deluge client and a Deluge server.</summary>
    public interface IExchangeMessage:
        IMessage
    {

        /// <summary>The identifier of the message which will allow the association of requests and replies.</summary>
        int Id { get; }
    }
}
