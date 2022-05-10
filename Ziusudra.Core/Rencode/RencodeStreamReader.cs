using System.Diagnostics;

namespace Ziusudra.Rencode
{

    /// <summary>Read Rencode formatted data from an underlying <see cref="Stream" />.</summary>
    public sealed class RencodeStreamReader:
        IRencodeReader,
        IDisposable
    {

        /// <summary>Create a new instance of the <see cref="RencodeStreamReader" /> type.</summary>
        /// <param name="stream">The underlying stream to read data from.</param>
        /// <remarks>The underlying <paramref name="stream" /> will be disposed when the current reader is disposed.</remarks>
        public RencodeStreamReader(Stream stream):
            this(stream, false)
        { }

        /// <summary>Create a new instance of the <see cref="RencodeStreamReader" /> type.</summary>
        /// <param name="stream">The underlying stream to read data from.</param>
        /// <param name="leaveOpen">Indicate whether the underlying stream should be left open when the current reader is disposed.</param>
        public RencodeStreamReader(Stream stream, bool leaveOpen)
        {
            _Stream = stream;
            _LeaveOpen = leaveOpen;
        }

        /// <summary>Create a new instance of the <see cref="RencodeStreamReader" /> type.</summary>
        /// <param name="buffer"></param>
        public RencodeStreamReader(byte[] buffer):
            this(new MemoryStream(buffer))
        { }

        /// <summary>Finalizer.</summary>
        ~RencodeStreamReader()
        {
            Dispose(false);
        }

        /// <summary>Closes the current reader.</summary>
        public void Close()
        {
            Dispose(true);
        }

        /// <summary>Read the next value.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="additionalEncoders">Encoders to use in addition to the standard ones.</param>
        /// <returns>The next value.</returns>
        /// <exception cref="RencodeException">Thrown when no encoder has been found to decode the value.</exception>
        public async ValueTask<object?> ReadValueAsync(CancellationToken cancellationToken, params IEncoder[] additionalEncoders)
        {
            IEnumerable<IEncoder> encoders = Encoder.Encoders;
            if (additionalEncoders.Length > 0)
                encoders= encoders.Union(additionalEncoders);

            byte header = (await ReadAsync(1, cancellationToken).ConfigureAwait(false))[0];
            foreach (var encoder in encoders)
                if (encoder.CanRead(header))
                    return await encoder.ReadValueAsync(this, header, cancellationToken).ConfigureAwait(false);

            throw new RencodeException("Invalid data");
        }

        internal async ValueTask<byte[]> ReadAsync(int length, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[length];
            int read = await _Stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            Debug.Assert(read == buffer.Length);
            if (read != buffer.Length)
                throw new RencodeException(SR.RencodeException_EmptyStream);

            return buffer;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ValueTask<byte[]> IRencodeReader.ReadAsync(int length, CancellationToken cancellationToken)
        {
            return ReadAsync(length, cancellationToken);
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
