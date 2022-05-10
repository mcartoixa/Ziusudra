using System.Collections;
using System.Diagnostics;
using System.IO.Compression;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Read Deluge RPC formatted messages from an underlying <see cref="Stream" />.</summary>
    public sealed class RpcStreamReader:
        IDisposable
    {

        /// <summary>Create a new instance of the <see cref="RpcStreamReader" /> type.</summary>
        /// <param name="stream">The underlying stream.</param>
        public RpcStreamReader(Stream stream):
            this(stream, false)
        { }

        /// <summary>Create a new instance of the <see cref="RpcStreamReader" /> type.</summary>
        /// <param name="stream">The underlying stream.</param>
        /// <param name="leaveOpen">Whether to leave the underlying stream open when the reader is disposed.</param>
        public RpcStreamReader(Stream stream, bool leaveOpen)
        {
            _Stream = stream;
            _LeaveOpen = leaveOpen;
        }

        /// <summary>Finalizer.</summary>
        ~RpcStreamReader()
        {
            Dispose(false);
        }

        /// <summary>Close the reader.</summary>
        public void Close()
        {
            ((IDisposable)this).Dispose();
        }

        /// <summary>Read the next message from the underlying stream using the Deluge RPC protocol.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The message that has been read.</returns>
        /// <exception cref="RpcException">Thrown when an error has been detected according to the Deluge RPC protocol.</exception>
        public async ValueTask<IServerMessage> ReadAsync(CancellationToken cancellationToken = default)
        {
            byte protocol = (await ReadBytesFromStreamAsync(1, cancellationToken).ConfigureAwait(false))[0];

            byte[] buffer = await ReadBytesFromStreamAsync(4, cancellationToken).ConfigureAwait(false);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            uint size = BitConverter.ToUInt32(buffer);

            using (var reader = new Rencode.RencodeStreamReader(new ZLibStream(_Stream, CompressionMode.Decompress, true), true))
            {
                var value = await reader.ReadValueAsync(cancellationToken).ConfigureAwait(false);
                // TODO: check that size was accurate?

                if (value is not ICollection collection)
                    throw new RpcException("A collection was expected");
                RpcMessageType type = (RpcMessageType)Convert.ToInt32(collection.Cast<object?>().FirstOrDefault());
                switch (type)
                {
                    case RpcMessageType.RPC_EVENT:
                        return new RpcEvent(collection);
                    case RpcMessageType.RPC_ERROR:
                        return new RpcServerException(collection);
                    case RpcMessageType.RPC_RESPONSE:
                        return new RpcResponse(collection);
                }
                throw new RpcException("Unrecognized message");
            }
        }

        internal async ValueTask<byte[]> ReadBytesFromStreamAsync(int length, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[length];
            int read = await _Stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            Debug.Assert(read == buffer.Length);
            if (read != buffer.Length)
                throw new RpcException(SR.RencodeException_EmptyStream);

            return buffer;
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
    }
}
