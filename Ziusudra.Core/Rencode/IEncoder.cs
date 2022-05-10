namespace Ziusudra.Rencode
{

    /// <summary>Interface implemented by a Rencode encoder.</summary>
    /// <remarks>An encoder can write and/or read specific values to and/or from the Rencode format.</remarks>
    public interface IEncoder
    {

        /// <summary>Indicate whether the current <see cref="IEncoder" /> can read the next value according to the specified <paramref name="header" />.</summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the current <see cref="IEncoder" /> can read the next value; or else <c>false</c>.</returns>
        bool CanRead(byte header);

        /// <summary>Indicate whether the current <see cref="IEncoder" /> can write the specified <paramref name="value" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the current <see cref="IEncoder" /> can write the specified <paramref name="value" />; or else <c>false</c>.</returns>
        bool CanWrite(object? value);

        /// <summary>Read a value from the current context of the specified <paramref name="reader" />.</summary>
        /// <param name="reader">The Rencode reader to read the value from.</param>
        /// <param name="header">The current header in the Rencode serialization.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The value that has been read.</returns>
        /// <remarks>The <paramref name="header" /> might encode the whole value, or only part of it in which case more bytes
        /// may have to be read from the specified <paramref name="reader"/>.</remarks>
        ValueTask<object?> ReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken);

        /// <summary>Write the specified <paramref name="value" /> to the specified <paramref name="writer" />.</summary>
        /// <param name="writer">The Rencode writer to write the </param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        ValueTask WriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken);
    }
}
