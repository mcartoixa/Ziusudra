namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="long" /> values.</summary>
    /// <remarks>Depending on their actual values, integers are encoded in the most appropriate format as defined by this encoder,
    /// <see cref="Int32Encoder" />, <see cref="Int16Encoder" /> and <see cref="SByteEncoder" />.</remarks>
    public sealed class Int64Encoder:
        Encoder<long>
    {

        /// <inheritdoc />
        /// <remarks>This method returns <c>true</c> if the value is encoded in any integer format, as defined by the current encoder or
        /// <see cref="Int32Encoder" />, <see cref="Int16Encoder" /> and <see cref="SByteEncoder" />.</remarks>
        public override bool CanRead(byte header)
        {
            return header == CHR_INT || Int32Encoder.Instance.CanRead(header);
        }

        /// <inheritdoc />
        protected async override ValueTask<long> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (header == CHR_INT)
            {
                byte[] buffer = await reader.ReadAsync(8, cancellationToken).ConfigureAwait(false);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                return BitConverter.ToInt64(buffer);
            }
            return await Int32Encoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, long value, CancellationToken cancellationToken)
        {
            if (value >= int.MinValue && value <= int.MaxValue)
            {
                await Int32Encoder.Instance.WriteValueAsync(writer, (int)value, cancellationToken).ConfigureAwait(false);
            } else
            {
                await writer.WriteAsync(new byte[] { CHR_INT }, cancellationToken).ConfigureAwait(false);
                byte[] buffer = BitConverter.GetBytes(value);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            }
        }

        internal static readonly Int64Encoder Instance = new();

        internal const byte CHR_INT = 65;
    }
}
