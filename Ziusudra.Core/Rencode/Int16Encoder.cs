namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="short" /> values.</summary>
    /// <remarks>Depending on their actual values, integers are encoded in the most appropriate format as defined by this encoder
    /// and <see cref="SByteEncoder" />.</remarks>
    public sealed class Int16Encoder:
        Encoder<short>
    {

        /// <inheritdoc />
        /// <remarks>This method returns <c>true</c> if the value is encoded in any integer format, as defined by the current encoder or
        /// <see cref="SByteEncoder" />.</remarks>
        public override bool CanRead(byte header)
        {
            return header == CHR_INT || SByteEncoder.Instance.CanRead(header);
        }

        /// <inheritdoc />
        protected async override ValueTask<short> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (header == CHR_INT)
            {
                byte[] buffer = await reader.ReadAsync(2, cancellationToken).ConfigureAwait(false);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                return BitConverter.ToInt16(buffer);
            }
            return await SByteEncoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, short value, CancellationToken cancellationToken)
        {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
            {
                await SByteEncoder.Instance.WriteValueAsync(writer, (sbyte)value, cancellationToken).ConfigureAwait(false);
            } else
            {
                await writer.WriteAsync(new byte[] { CHR_INT }, cancellationToken).ConfigureAwait(false);
                byte[] buffer = BitConverter.GetBytes(value);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            }
        }

        internal static readonly Int16Encoder Instance = new();

        internal const byte CHR_INT = 63;
    }
}
