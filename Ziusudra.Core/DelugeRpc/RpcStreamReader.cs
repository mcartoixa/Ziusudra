using System.Collections;
using System.Globalization;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Read Deluge RPC formatted messages from an underlying <see cref="Stream" />.</summary>
    public sealed class RpcStreamReader:
        IServerMessageReader,
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
        /// <exception cref="EndOfStreamException">Thrown when the underlying stream ends before a complete frame has been read.</exception>
        /// <exception cref="RpcException">Thrown when an error has been detected according to the Deluge RPC protocol.</exception>
        public async ValueTask<IServerMessage> ReadAsync(CancellationToken cancellationToken = default)
        {
            byte version = (await ReadExactlyAsync(1, cancellationToken).ConfigureAwait(false))[0];
            if (version != PROTOCOL_VERSION)
                throw new RpcException(string.Format(CultureInfo.CurrentCulture, SR.RpcException_InvalidProtocolVersion, version, PROTOCOL_VERSION));

            byte[] header = await ReadExactlyAsync(4, cancellationToken)
                .ConfigureAwait(false);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(header);
            uint size = BitConverter.ToUInt32(header);
            if (size > (uint)MaxFrameSize)
                throw new RpcException(string.Format(CultureInfo.CurrentCulture, SR.RpcException_MessageTooLarge, size, MaxFrameSize));

            // Read the whole compressed frame so decompression is bounded to this message
            // and cannot over-read into the next one on the underlying stream.
            byte[] body = await ReadExactlyAsync((int)size, cancellationToken)
                .ConfigureAwait(false);

            object? content;
            using (var compressed = new MemoryStream(body, false))
            using (var decompressed = new ZLibStream(compressed, CompressionMode.Decompress))
            using (var reader = new Rencode.RencodeStreamReader(decompressed, true))
            {
                content = await reader.ReadValueAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            Logger.LogTrace("Deluge RPC message read: {Message}", MessageExtensions.ToDebugString(content));

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

        private async ValueTask<byte[]> ReadExactlyAsync(int length, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[length];
            await _Stream.ReadExactlyAsync(buffer, cancellationToken)
                .ConfigureAwait(false);
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

        /// <summary>Gets or sets the maximum accepted size, in bytes, of a single compressed message frame.</summary>
        /// <remarks>A frame whose announced size exceeds this value is rejected before any allocation, guarding against a
        /// corrupt or hostile length prefix. Deluge itself does not cap the frame size.</remarks>
        public int MaxFrameSize { get; set; } = DefaultMaxFrameSize;

        private readonly bool _LeaveOpen;
        private ILogger _Logger = NullLogger.Instance;
        private readonly Stream _Stream;

        /// <summary>The default value of <see cref="MaxFrameSize" />.</summary>
        public const int DefaultMaxFrameSize = 256 * 1024 * 1024;

        private const byte PROTOCOL_VERSION = 1;
    }
}
