using System.Globalization;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to integer values of any type.</summary>
    public class IntegerEncoder:
        Encoder
    {

        /// <summary>Indicate whether the current <see cref="NullEncoder" /> can read the next value according to the specified <paramref name="header" />.</summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the specified <paramref name="header" /> encodes for an integer; or else <c>false</c>.</returns>
        public override bool CanRead(byte header)
        {
            return BigIntegerEncoder.Instance.CanRead(header);
        }

        /// <summary>Indicate whether the current <see cref="NullEncoder" /> can write the specified <paramref name="value" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified <paramref name="value" /> is an integer; or else <c>false</c>.</returns>
        public override bool CanWrite(object? value)
        {
            return value is sbyte ||
                value is byte ||
                value is short ||
                value is ushort ||
                value is int ||
                value is uint ||
                value is long ||
                value is ulong;
        }

        /// <inheritdoc />
        protected override async ValueTask<object?> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (SByteEncoder.Instance.CanRead(header))
                return await SByteEncoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
            else if (Int16Encoder.Instance.CanRead(header))
                return await Int16Encoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
            else if (Int32Encoder.Instance.CanRead(header))
                return await Int32Encoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
            else if (Int64Encoder.Instance.CanRead(header))
                return await Int64Encoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);
            else if (BigIntegerEncoder.Instance.CanRead(header))
                return await BigIntegerEncoder.Instance.ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false);

            throw new RencodeException(
                string.Format(CultureInfo.CurrentCulture, SR.RencodeException_EncoderCannotReadThisHeader, GetType().Name, header)
            );
        }

        /// <inheritdoc />
        protected override async ValueTask DoWriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken)
        {
            switch (value)
            {
                case sbyte v:
                    await SByteEncoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case byte v:
                    await Int16Encoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case short v:
                    await Int16Encoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case ushort v:
                    await Int32Encoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case int v:
                    await Int32Encoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case uint v:
                    await Int64Encoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case long v:
                    await Int64Encoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case ulong v:
                    await BigIntegerEncoder.Instance.WriteValueAsync(writer, v, cancellationToken).ConfigureAwait(false);
                    break;
                case null:
                    await NullEncoder.Instance.WriteValueAsync(writer, null, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new RencodeException(
                        string.Format(CultureInfo.CurrentCulture, SR.RencodeException_EncoderCannotWriteThisValue, GetType().Name, value, value.GetType().Name)
                    );
            }
        }

        internal static readonly IntegerEncoder Instance = new();
    }
}
