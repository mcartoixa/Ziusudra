namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="double" /> values.</summary>
    public sealed class DoubleEncoder:
        Encoder<double>
    {

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return header == CHR_FLOAT;
        }

        /// <inheritdoc />
        protected async override ValueTask<double> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (header == CHR_FLOAT)
            {
                byte[] buffer = await reader.ReadAsync(8, cancellationToken).ConfigureAwait(false);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                return BitConverter.ToDouble(buffer);
            }
            return await SingleEncoder.Instance
                .ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, double value, CancellationToken cancellationToken)
        {
            await writer.WriteAsync(new byte[] { CHR_FLOAT }, cancellationToken).ConfigureAwait(false);
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        internal static readonly DoubleEncoder Instance = new();

        internal const byte CHR_FLOAT = 44;
    }
}
