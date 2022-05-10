using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ziusudra.Rencode.Tests
{

    internal class FakeMemoryStreamWriter:
        IRencodeWriter
    {

        public FakeMemoryStreamWriter()
        {
            MemoryStream = new MemoryStream();
        }

        public ValueTask WriteValueAsync(object? value, CancellationToken cancellationToken, params IEncoder[] additionalEncoders)
        {
            return WriteAsync(new byte[] { DEFAULT_VALUE }, cancellationToken);
        }

        public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            return MemoryStream.WriteAsync(buffer, cancellationToken);
        }

        public byte[] Buffer => MemoryStream.ToArray();

        private MemoryStream MemoryStream { get; set; }

        internal const byte DEFAULT_VALUE = 0x01;
    }
}
