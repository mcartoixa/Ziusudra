using System.Globalization;

namespace Ziusudra.Rencode
{

    /// <summary>Write Rencode formatted data to an underlying <see cref="Stream" />.</summary>
    public sealed class RencodeStreamWriter:
        IRencodeWriter,
        IDisposable
    {

        /// <summary>Create a new instance of the <see cref="RencodeStreamWriter" /> type.</summary>
        /// <param name="stream">The underlying stream on which to write.</param>
        /// <remarks>The underlying <paramref name="stream" /> will be disposed when the current writer is disposed.</remarks>
        public RencodeStreamWriter(Stream stream):
            this(stream, false)
        { }

        /// <summary>Create a new instance of the <see cref="RencodeStreamWriter" /> type.</summary>
        /// <param name="stream">The underlying stream on which to write.</param>
        /// <param name="leaveOpen">Indicate whether the underlying stream should be left open when the current writer is disposed.</param>
        public RencodeStreamWriter(Stream stream, bool leaveOpen)
        {
            _Stream = stream;
            _LeaveOpen = leaveOpen;
        }

        /// <summary>Finalizer.</summary>
        ~RencodeStreamWriter()
        {
            Dispose(false);
        }

        /// <summary>Closes the current writer.</summary>
        public void Close()
        {
            Dispose(true);
        }

        /// <summary>Write the specified <paramref name="value" /> in the Rencode format to the underlying stream.</summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="additionalEncoders">Encoders to use in addition to the standard ones.</param>
        /// <exception cref="RencodeException">Thrown when no encoder has been found to encode the value.</exception>
        public ValueTask WriteValueAsync(object? value, CancellationToken cancellationToken, params IEncoder[] additionalEncoders)
        {
            IEnumerable<IEncoder> encoders = Encoder.Encoders;
            if (additionalEncoders.Length > 0)
                encoders = encoders.Union(additionalEncoders);

            foreach (var encoder in encoders)
                if (encoder.CanWrite(value))
                    return encoder.WriteValueAsync(this, value, cancellationToken);

            throw new RencodeException(
                string.Format(CultureInfo.CurrentCulture, SR.RencodeException_NoEncoderFound, value, value?.GetType().Name)
            );
        }

        internal ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            return _Stream.WriteAsync(buffer, cancellationToken);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ValueTask IRencodeWriter.WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            return WriteAsync(buffer, cancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_LeaveOpen)
                    _Stream.Dispose();
            }
        }

        private readonly Stream _Stream;
        private readonly bool _LeaveOpen;
    }
}
