using System.Diagnostics;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="bool" /> values.</summary>
    public sealed class BooleanEncoder:
        Encoder<bool>
    {

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return header == CHR_TRUE || header == CHR_FALSE;
        }

        /// <inheritdoc />
        protected override ValueTask<bool> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            Debug.Assert(header == CHR_TRUE || header == CHR_FALSE);
            return ValueTask.FromResult(header == CHR_TRUE);
        }

        /// <inheritdoc />
        protected override ValueTask DoWriteValueAsync(IRencodeWriter writer, bool value, CancellationToken cancellationToken)
        {
            return writer.WriteAsync(new byte[] { value ? CHR_TRUE : CHR_FALSE }, cancellationToken);
        }

        internal static readonly BooleanEncoder Instance = new();

        internal const byte CHR_TRUE = 67;
        internal const byte CHR_FALSE = 68;
    }
}
