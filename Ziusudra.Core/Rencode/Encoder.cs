using System.Globalization;

namespace Ziusudra.Rencode
{

    /// <summary>Base implementation of a Rencode encoder.</summary>
    /// <remarks>An encoder can write and/or read specific values to and/or from the Rencode format.</remarks>
    public abstract class Encoder:
        IEncoder
    {

        /// <summary>Indicate whether the current <see cref="Encoder" /> can read the next value according to the specified <paramref name="header" />.</summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the current <see cref="Encoder" /> can read the next value; or else <c>false</c>.</returns>
        public virtual bool CanRead(byte header)
        {
            return false;
        }

        /// <summary>Indicate whether the current <see cref="Encoder" /> can write the specified <paramref name="value" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the current <see cref="Encoder" /> can write the specified <paramref name="value"/>; or else <c>false</c>.</returns>
        public virtual bool CanWrite(object? value)
        {
            return false;
        }

        /// <summary>Read a value from the current context of the specified <paramref name="reader" />.</summary>
        /// <param name="reader">The Rencode reader to read the value from.</param>
        /// <param name="header">The current header in the Rencode serialization.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The value that has been read.</returns>
        public ValueTask<object?> ReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (!CanRead(header))
                throw new RencodeException(
                    string.Format(CultureInfo.CurrentCulture, SR.RencodeException_EncoderCannotReadThisHeader, GetType().Name, header)
                );
            return DoReadValueAsync(reader, header, cancellationToken);
        }

        /// <summary>Write the specified <paramref name="value" /> to the specified <paramref name="writer" />.</summary>
        /// <param name="writer">The Rencode writer to write the </param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public ValueTask WriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken)
        {
            if (!CanWrite(value))
                throw new RencodeException(
                    string.Format(CultureInfo.CurrentCulture, SR.RencodeException_EncoderCannotWriteThisValue, GetType().Name, value, value?.GetType().Name)
                );
            return DoWriteValueAsync(writer, value, cancellationToken);
        }

        /// <summary>Read a value from the current context of the specified <paramref name="reader" />.</summary>
        /// <param name="reader">The Rencode reader to read the value from.</param>
        /// <param name="header">The current header in the Rencode serialization.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The value that has been read.</returns>
        /// <remarks>The <paramref name="header" /> might encode the whole value, or only part of it in which case more bytes
        /// may have to be read from the specified <paramref name="reader"/>.</remarks>
        protected abstract ValueTask<object?> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken);

        /// <summary>Write the specified <paramref name="value" /> to the specified <paramref name="writer" />.</summary>
        /// <param name="writer">The Rencode writer to write the </param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        protected abstract ValueTask DoWriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken);

        /// <summary>The list of all standard Rencode encoders.</summary>
        internal protected static readonly IEnumerable<IEncoder> Encoders = new IEncoder[] {
            NullEncoder.Instance,
            BooleanEncoder.Instance,
            SByteEncoder.Instance,
            Int16Encoder.Instance,
            Int32Encoder.Instance,
            Int64Encoder.Instance,
            BigIntegerEncoder.Instance,
            SingleEncoder.Instance,
            DoubleEncoder.Instance,
            StringEncoder.Instance,
            DictionaryEncoder.Instance, // *Must* appear before ListEncoder, as IDictionary is also an ICollection
            ListEncoder.Instance,
            IntegerEncoder.Instance // Only used for writing
        };
    }

    /// <summary>Base implementation of a typed Rencode encoder.</summary>
    /// <remarks>An encoder can write and/or read values of type <typeparamref name="T" /> to and/or from the Rencode format.</remarks>
    public abstract class Encoder<T>:
        IEncoder
    {

        /// <summary>Indicate whether the current <see cref="Encoder{T}" /> can read the next value according to the specified <paramref name="header" />.</summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the current <see cref="Encoder{T}" /> can read the next value; or else <c>false</c>.</returns>
        public virtual bool CanRead(byte header)
        {
            return false;
        }

        /// <summary>Indicate whether the current <see cref="Encoder" /> can write the specified <paramref name="value" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the current <see cref="Encoder" /> can write the specified <paramref name="value"/>; or else <c>false</c>.</returns>
        public virtual bool CanWrite(T value)
        {
            return true;
        }

        /// <summary>Read a value from the current context of the specified <paramref name="reader" />.</summary>
        /// <param name="reader">The Rencode reader to read the value from.</param>
        /// <param name="header">The current header in the Rencode serialization.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The value that has been read.</returns>
        public ValueTask<T> ReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            if (!CanRead(header))
                throw new RencodeException(
                    string.Format(CultureInfo.CurrentCulture, SR.RencodeException_EncoderCannotReadThisHeader, GetType().Name, header)
                );
            return DoReadValueAsync(reader, header, cancellationToken);
        }

        /// <summary>Write the specified <paramref name="value" /> to the specified <paramref name="writer" />.</summary>
        /// <param name="writer">The Rencode writer to write the </param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public ValueTask WriteValueAsync(IRencodeWriter writer, T? value, CancellationToken cancellationToken)
        {
            if (value != null)
            {
                if (!((IEncoder)this).CanWrite(value))
                    throw new RencodeException(
                        string.Format(CultureInfo.CurrentCulture, SR.RencodeException_EncoderCannotWriteThisValue, GetType().Name, value, value.GetType().Name)
                    );
                return DoWriteValueAsync(writer, value, cancellationToken);
            }
            else
                return NullEncoder.Instance.WriteValueAsync(writer, value, cancellationToken);
        }

        /// <summary>Read a value from the current context of the specified <paramref name="reader" />.</summary>
        /// <param name="reader">The Rencode reader to read the value from.</param>
        /// <param name="header">The current header in the Rencode serialization.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The value that has been read.</returns>
        /// <remarks>The <paramref name="header" /> might encode the whole value, or only part of it in which case more bytes
        /// may have to be read from the specified <paramref name="reader"/>.</remarks>
        protected abstract ValueTask<T> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken);

        /// <summary>Write the specified <paramref name="value" /> to the specified <paramref name="writer" />.</summary>
        /// <param name="writer">The Rencode writer to write the </param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        protected abstract ValueTask DoWriteValueAsync(IRencodeWriter writer, T value, CancellationToken cancellationToken);

        bool IEncoder.CanWrite(object? value)
        {
            return value != null && value is T t && CanWrite(t);
        }

        async ValueTask<object?> IEncoder.ReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            return await ReadValueAsync(reader, header, cancellationToken).ConfigureAwait(false); // No straightforward way to convert except await...
        }

        ValueTask IEncoder.WriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken)
        {
            return WriteValueAsync(writer, (T?)value, cancellationToken);
        }
    }
}
