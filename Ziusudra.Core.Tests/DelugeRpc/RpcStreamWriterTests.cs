using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.DelugeRpc.Tests
{

    public class RpcStreamWriterTests
    {

        [Fact]
        public async Task WriteAsync_ShouldSerializeConcurrentWritesToTheStream()
        {
            // Regression test: RpcClient allows several requests in flight at once, so concurrent WriteAsync calls
            // race on the shared stream. Without serialization their frames interleave (and a real SslStream throws
            // "another write operation is pending"). Every flush must own the stream for the whole frame.
            using var stream = new OverlapTrackingStream();
            using var writer = new RpcStreamWriter(stream, true);

            const int concurrency = 50;
            var writes = new Task[concurrency];
            for (int i = 0; i < concurrency; i++)
                writes[i] = Task.Run(() => writer.WriteAsync(new GetExternalIpRequest()).AsTask());
            await Task.WhenAll(writes);

            Assert.False(stream.OverlapObserved);
            Assert.Equal(1, stream.MaxConcurrency);
            Assert.Equal(concurrency, CountFrames(stream.Written.ToArray()));
        }

        private static int CountFrames(byte[] data)
        {
            int count = 0;
            int position = 0;
            while (position < data.Length)
            {
                Assert.Equal(PROTOCOL_VERSION, data[position]);
                byte[] length = data[(position + 1)..(position + HEADER_SIZE)];
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);
                position += HEADER_SIZE + (int)BitConverter.ToUInt32(length);
                count++;
            }
            Assert.Equal(data.Length, position);
            return count;
        }

        private const byte PROTOCOL_VERSION = 1;
        private const int HEADER_SIZE = 5;
    }

    /// <summary>A write-only stream that records the largest number of writes it saw in flight at once, mimicking an
    /// <see cref="System.Net.Security.SslStream" />'s intolerance of overlapping writes without throwing.</summary>
    internal sealed class OverlapTrackingStream:
        Stream
    {

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int active = Interlocked.Increment(ref _Active);
            lock (_Sync)
            {
                MaxConcurrency = Math.Max(MaxConcurrency, active);
                if (active > 1)
                    OverlapObserved = true;
            }
            try
            {
                // Widen the window so a missing lock is observed deterministically rather than by luck of scheduling.
                await Task.Yield();
                lock (_Sync)
                    Written.Write(buffer.Span);
            } finally
            {
                Interlocked.Decrement(ref _Active);
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_Sync)
                Written.Write(buffer, offset, count);
        }

        public MemoryStream Written { get; } = new();
        public int MaxConcurrency { get; private set; }
        public bool OverlapObserved { get; private set; }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => Written.Length;
        public override long Position { get => Written.Position; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Written.Dispose();
            base.Dispose(disposing);
        }

        private int _Active;
        private readonly object _Sync = new();
    }
}
