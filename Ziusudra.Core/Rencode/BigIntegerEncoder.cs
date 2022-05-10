using System.Globalization;
using System.Numerics;
using System.Text;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="BigInteger" /> values.</summary>
    /// <remarks>Depending on their actual values, integers are encoded in the most appropriate format as defined by this encoder,
    /// <see cref="Int64Encoder" />, <see cref="Int32Encoder" />, <see cref="Int16Encoder" /> and <see cref="SByteEncoder" />.</remarks>
    public sealed class BigIntegerEncoder:
        Encoder<BigInteger>
    {

        /// <inheritdoc />
        /// <remarks>This method returns <c>true</c> if the value is encoded in any integer format, as defined by the current encoder or
        /// <see cref="Int64Encoder" />, <see cref="Int32Encoder" />, <see cref="Int16Encoder" /> or <see cref="SByteEncoder" />.</remarks>
        public override bool CanRead(byte header)
        {
            return header == CHR_INT || Int64Encoder.Instance.CanRead(header);
        }

        /// <inheritdoc />
        protected async override ValueTask<BigInteger> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (header == CHR_INT)
            {
                byte current = (await reader.ReadAsync(1, cancellationToken).ConfigureAwait(false))[0];
                StringBuilder sb = new ();
                while (current != CHR_TERM)
                {
                    sb.Append((char)current);
                    current = (await reader.ReadAsync(1, cancellationToken).ConfigureAwait(false))[0];
                }
                return BigInteger.Parse(sb.ToString(), CultureInfo.InvariantCulture);
            }
            return await Int64Encoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, BigInteger value, CancellationToken cancellationToken)
        {
            if (value >= long.MinValue && value <= long.MaxValue)
            {
                await Int64Encoder.Instance.WriteValueAsync(writer, (long)value, cancellationToken).ConfigureAwait(false);
            } else
            {
                await writer.WriteAsync(new byte[] { CHR_INT }, cancellationToken).ConfigureAwait(false);
                await writer.WriteAsync(Encoding.ASCII.GetBytes(value.ToString("D")), cancellationToken).ConfigureAwait(false);
                await writer.WriteAsync(new byte[] { CHR_TERM }, cancellationToken).ConfigureAwait(false);
            }
        }

        internal static readonly BigIntegerEncoder Instance = new();

        internal const byte CHR_INT = 61;
        internal const byte CHR_TERM = 127;
    }
}
