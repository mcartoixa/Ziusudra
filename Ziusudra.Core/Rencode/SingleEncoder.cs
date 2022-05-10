namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="float" /> values.</summary>
    public sealed class SingleEncoder:
        Encoder<float>
    {

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return header == CHR_FLOAT;
        }

        /// <inheritdoc />
        protected async override ValueTask<float> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (header == CHR_FLOAT)
            {
                byte[] buffer = await reader.ReadAsync(4, cancellationToken).ConfigureAwait(false);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                return BitConverter.ToSingle(buffer);
            }

            throw new RencodeException("Invalid value");
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, float value, CancellationToken cancellationToken)
        {
            await writer.WriteAsync(new byte[] { CHR_FLOAT }, cancellationToken).ConfigureAwait(false);
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        internal static readonly SingleEncoder Instance = new();

        internal const byte CHR_FLOAT = 66;
    }
}
