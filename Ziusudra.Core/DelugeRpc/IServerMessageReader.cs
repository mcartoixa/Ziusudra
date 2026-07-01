namespace Ziusudra.DelugeRpc
{

    /// <summary>Reads <see cref="IServerMessage" /> instances sent by a Deluge server.</summary>
    internal interface IServerMessageReader
    {

        /// <summary>Read the next message sent by the server.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The message that has been read.</returns>
        ValueTask<IServerMessage> ReadAsync(CancellationToken cancellationToken = default);
    }
}
