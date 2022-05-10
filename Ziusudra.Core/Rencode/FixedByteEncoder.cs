namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to fixed <see cref="byte" /> values.</summary>
    /// <remarks>This encoder can be used to encode and decode a separator code for instance.</remarks>
    public class FixedByteEncoder:
        Encoder
    {

        /// <summary>Create a new instance of the <see cref="FixedByteEncoder" /> type.</summary>
        /// <param name="value">The fixed value this encoder will handle.</param>
        public FixedByteEncoder(byte value)
        {
            _Value = value;
        }

        /// <summary>Indicate whether the current encoder can read the specified <paramref name="header" />.</summary>
        /// <param name="header">The header to read.</param>
        /// <returns><c>true</c> if the specified <paramref name="header" /> is the fixed value of the current encoder; or else <c>false</c>.</returns>
        public override bool CanRead(byte header)
        {
            return header == _Value;
        }

        /// <summary>Indicate whether the current encoder can write the specified <paramref name="value" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified <paramref name="value" /> is the fixed value of the current encoder; or else <c>false</c>.</returns>
        public override bool CanWrite(object? value)
        {
            return value is byte b && b == _Value;
        }

        /// <inheritdoc />
        protected override ValueTask<object?> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult<object?>(_Value);
        }

        /// <inheritdoc />
        protected override ValueTask DoWriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken)
        {
            return writer.WriteAsync(new byte[] { _Value }, cancellationToken);
        }

        private readonly byte _Value;
    }
}
