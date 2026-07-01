using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Ziusudra.Rencode;

namespace Ziusudra.DelugeRpc.Tests
{

    public class RpcStreamReaderTests
    {

        [Fact]
        public async Task ReadAsync_ShouldDecodeAResponse()
        {
            byte[] frame = await FrameResponseAsync(42, "ok");
            using var reader = new RpcStreamReader(new MemoryStream(frame));

            IServerMessage message = await reader.ReadAsync(WithTimeout());

            RpcResponse response = Assert.IsType<RpcResponse>(message);
            Assert.Equal(42, response.Id);
        }

        [Fact]
        public async Task ReadAsync_ShouldDecodeConsecutiveFramesFromTheSameStream()
        {
            // Regression test for the ZLib over-read desync: back-to-back frames must both decode.
            byte[] stream = Concat(
                await FrameResponseAsync(1, "first"),
                await FrameResponseAsync(2, "second"));
            using var reader = new RpcStreamReader(new MemoryStream(stream));

            RpcResponse first = Assert.IsType<RpcResponse>(await reader.ReadAsync(WithTimeout()));
            RpcResponse second = Assert.IsType<RpcResponse>(await reader.ReadAsync(WithTimeout()));

            Assert.Equal(1, first.Id);
            Assert.Equal(2, second.Id);
        }

        [Fact]
        public async Task ReadAsync_ShouldDecodeWhenTheUnderlyingStreamYieldsOneByteAtATime()
        {
            byte[] stream = Concat(
                await FrameResponseAsync(1, "first"),
                await FrameResponseAsync(2, "second"));
            using var reader = new RpcStreamReader(new ChunkedStream(stream, 1));

            RpcResponse first = Assert.IsType<RpcResponse>(await reader.ReadAsync(WithTimeout()));
            RpcResponse second = Assert.IsType<RpcResponse>(await reader.ReadAsync(WithTimeout()));

            Assert.Equal(1, first.Id);
            Assert.Equal(2, second.Id);
        }

        [Fact]
        public async Task ReadAsync_ShouldThrowWhenTheStreamIsTruncated()
        {
            byte[] frame = await FrameResponseAsync(1, "truncated");
            byte[] truncated = frame[..^3];
            using var reader = new RpcStreamReader(new MemoryStream(truncated));

            await Assert.ThrowsAsync<EndOfStreamException>(async () => await reader.ReadAsync(WithTimeout()));
        }

        [Fact]
        public async Task ReadAsync_ShouldRejectAFrameLargerThanTheMaximum()
        {
            byte[] frame = await FrameResponseAsync(1, "too large");
            using var reader = new RpcStreamReader(new MemoryStream(frame)) { MaxFrameSize = 1 };

            await Assert.ThrowsAsync<RpcException>(async () => await reader.ReadAsync(WithTimeout()));
        }

        [Fact]
        public async Task ReadAsync_ShouldRejectAnUnknownProtocolVersion()
        {
            using var reader = new RpcStreamReader(new MemoryStream(new byte[] { 2, 0, 0, 0, 0 }));

            await Assert.ThrowsAsync<RpcException>(async () => await reader.ReadAsync(WithTimeout()));
        }

        private static CancellationToken WithTimeout()
        {
            return new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
        }

        private static byte[] Concat(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        private static async Task<byte[]> FrameResponseAsync(int id, object result)
        {
            var values = new ArrayList { (int)RpcMessageType.RPC_RESPONSE, id, result };

            using var ms = new MemoryStream();
            ms.WriteByte(PROTOCOL_VERSION);
            ms.SetLength(HEADER_SIZE);
            ms.Position = HEADER_SIZE;

            using (var compressed = new ZLibStream(ms, CompressionMode.Compress, true))
            using (var writer = new RencodeStreamWriter(compressed, true))
                await writer.WriteValueAsync(values, CancellationToken.None);

            ms.Position = 1;
            byte[] size = BitConverter.GetBytes((uint)(ms.Length - HEADER_SIZE));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(size);
            ms.Write(size);

            return ms.ToArray();
        }

        private const byte PROTOCOL_VERSION = 1;
        private const int HEADER_SIZE = 5;
    }

    /// <summary>A read-only stream that never returns more than <c>maxRead</c> bytes per read, exercising partial-read handling.</summary>
    internal sealed class ChunkedStream:
        Stream
    {

        public ChunkedStream(byte[] data, int maxRead)
        {
            _Data = data;
            _MaxRead = maxRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_Position >= _Data.Length)
                return 0;
            int toCopy = Math.Min(Math.Min(count, _MaxRead), _Data.Length - _Position);
            Buffer.BlockCopy(_Data, _Position, buffer, offset, toCopy);
            _Position += toCopy;
            return toCopy;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_Position >= _Data.Length)
                return ValueTask.FromResult(0);
            int toCopy = Math.Min(Math.Min(buffer.Length, _MaxRead), _Data.Length - _Position);
            _Data.AsSpan(_Position, toCopy).CopyTo(buffer.Span);
            _Position += toCopy;
            return ValueTask.FromResult(toCopy);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _Data.Length;
        public override long Position { get => _Position; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        private readonly byte[] _Data;
        private readonly int _MaxRead;
        private int _Position;
    }
}
