using System.IO.Compression;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Write Deluge RPC formatted requests to an underlying <see cref="Stream" />.</summary>
    public sealed class RpcStreamWriter:
        IDisposable
    {

        /// <summary>Create a new instance of the <see cref="RpcStreamWriter" /> type.</summary>
        /// <param name="stream">The underlying stream on which to write.</param>
        /// <remarks>The underlying <paramref name="stream" /> will be disposed when the current writer is disposed.</remarks>
        public RpcStreamWriter(Stream stream):
            this(stream, false)
        { }

        /// <summary>Create a new instance of the <see cref="RpcStreamWriter" /> type.</summary>
        /// <param name="stream">The underlying stream on which to write.</param>
        /// <param name="leaveOpen">Indicate whether the underlying stream should be left open when the current writer is disposed.</param>
        public RpcStreamWriter(Stream stream, bool leaveOpen)
        {
            _Stream = stream;
            _LeaveOpen = leaveOpen;
        }

        /// <summary>Finalizer.</summary>
        ~RpcStreamWriter()
        {
            Dispose(false);
        }

        /// <summary>Close the current writer.</summary>
        public void Close()
        {
            ((IDisposable)this).Dispose();
        }

        /// <summary>Write the specified <paramref name="request" /> to the underlying stream using the Deluge RPC protocol.</summary>
        /// <param name="request">The request to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async ValueTask WriteAsync(IClientRequest request, CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(PROTOCOL_VERSION);

            // Leave space for the header
            ms.SetLength(5);
            ms.Position = 5;

            // Write the content
            using (var contentStream = new ZLibStream(ms, CompressionMode.Compress, true))
            {
                using var writer = new Rencode.RencodeStreamWriter(contentStream, true);
                await writer.WriteValueAsync(request.ToValueCollection(), cancellationToken);
            }

            // Now write the length in the header
            ms.Position = 1;
            var length = BitConverter.GetBytes((uint)(ms.Length - 5));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(length);
            ms.Write(length);

            ms.Position = 0;
            await ms.CopyToAsync(_Stream, cancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_LeaveOpen)
                    _Stream.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private readonly Stream _Stream;
        private readonly bool _LeaveOpen;

        private const byte PROTOCOL_VERSION = 1;
    }
}
