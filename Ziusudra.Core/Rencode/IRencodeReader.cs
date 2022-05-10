namespace Ziusudra.Rencode
{

    /// <summary>Interface implemented by a Rencode reader.</summary>
    public interface IRencodeReader
    {

        /// <summary>Read the next value.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="additionalEncoders">Encoders to use in addition to the standard ones.</param>
        /// <returns>The next value.</returns>
        ValueTask<object?> ReadValueAsync(CancellationToken cancellationToken, params IEncoder[] additionalEncoders);

        /// <summary>Read the next sequence of bytes of the specified <paramref name="length"/>.</summary>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The sequence of bytes.</returns>
        ValueTask<byte[]> ReadAsync(int length, CancellationToken cancellationToken);
    }
}
