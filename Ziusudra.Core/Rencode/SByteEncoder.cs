namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="sbyte" /> values.</summary>
    public sealed class SByteEncoder:
        Encoder<sbyte>
    {

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return (header == CHR_INT) ||
                (header >= NEG_FIXED_START && header < NEG_FIXED_START + NEG_FIXED_COUNT) ||
                (header >= POS_FIXED_START && header < POS_FIXED_START + POS_FIXED_COUNT);
        }

        /// <inheritdoc />
        protected async override ValueTask<sbyte> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (header == CHR_INT)
            {
                return (sbyte)(await reader.ReadAsync(1, cancellationToken).ConfigureAwait(false))[0];
            } else if (header >= POS_FIXED_START && header < POS_FIXED_START + POS_FIXED_COUNT)
            {
                return (sbyte)((sbyte)header - POS_FIXED_START);
            } else if (header >= NEG_FIXED_START && header < NEG_FIXED_START + NEG_FIXED_COUNT)
            {
                return (sbyte)((sbyte)header + NEG_FIXED_START);
            }

            throw new RencodeException("Invalid value");
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, sbyte value, CancellationToken cancellationToken)
        {
            if (value >= 0 && value < POS_FIXED_COUNT)
            {
                await writer.WriteAsync(new byte[] { (byte)(POS_FIXED_START + value) }, cancellationToken).ConfigureAwait(false);
            } else if (value >= -NEG_FIXED_COUNT && value < 0)
            {
                await writer.WriteAsync(new byte[] { (byte)(NEG_FIXED_START - 1 - value) }, cancellationToken).ConfigureAwait(false);
            } else
            {
                await writer.WriteAsync(new byte[] { CHR_INT }, cancellationToken).ConfigureAwait(false);
                await writer.WriteAsync(new byte[] { (byte)value }, cancellationToken).ConfigureAwait(false);
            }
        }

        internal static readonly SByteEncoder Instance = new();

        internal const byte CHR_INT = 62;
        internal const byte NEG_FIXED_COUNT = 32;
        internal const byte NEG_FIXED_START = 70;
        internal const byte POS_FIXED_START = 0;
        internal const byte POS_FIXED_COUNT = 44;
    }
}
