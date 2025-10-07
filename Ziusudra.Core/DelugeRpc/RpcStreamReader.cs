using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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

            byte[] buffer = await ReadBytesFromStreamAsync(4, cancellationToken)
                .ConfigureAwait(false);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            uint size = BitConverter.ToUInt32(buffer);

            using (var reader = new Rencode.RencodeStreamReader(new ZLibStream(_Stream, CompressionMode.Decompress, true), true))
            {
                var content = await reader.ReadValueAsync(cancellationToken)
                    .ConfigureAwait(false);
                Logger.LogTrace("Deluge RPC message read: {0}", MessageExtensions.ToDebugString(content));
                // TODO: check that size was accurate?

                if (content is not ICollection values)
                    throw new RpcException(string.Format(CultureInfo.CurrentCulture, SR.RpcException_CollectionWasExpected, content?.GetType().ToString() ?? "<null>"));
                RpcMessageType type = (RpcMessageType)Convert.ToInt32(values.Cast<object?>().FirstOrDefault());
                switch (type)
                {
                    case RpcMessageType.RPC_EVENT:
                        return RpcEvent.CreateFromValues(values);
                    case RpcMessageType.RPC_ERROR:
                        return RpcServerException.CreateFromValues(values);
                    case RpcMessageType.RPC_RESPONSE:
                        return new RpcResponse(values);
                }
                throw new RpcException(string.Format(CultureInfo.CurrentCulture, SR.RpcException_InvalidMessageType, type));
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

        /// <summary>Get or set the current logger.</summary>
        public ILogger Logger
        {
            get
            {
                return _Logger;
            }
            set
            {
                _Logger = value;
            }
        }

        private readonly bool _LeaveOpen;
        private ILogger _Logger = NullLogger.Instance;
        private readonly Stream _Stream;
    }
}
