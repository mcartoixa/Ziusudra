namespace Ziusudra.Rencode
{

    /// <summary>Interface implemented by a Rencode writer.</summary>
    public interface IRencodeWriter
    {

        /// <summary>Write the specified <paramref name="value" /> in the Rencode format.</summary>
        /// <param name="value">The value to be written.</param>
        /// <param name="additionalEncoders">Encoders to use in addition to the standard ones.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        ValueTask WriteValueAsync(object? value, CancellationToken cancellationToken, params IEncoder[] additionalEncoders);

        /// <summary>Write the specified <paramref name="buffer" /> without encoding.</summary>
        /// <param name="buffer">The buffer to be written.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <remarks>It is the responsibility of the caller of this method to make sure that the result will still be
        /// a valid Rencode format.</remarks>
        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
    }
}
