using System.Threading;
using Xunit;

namespace Ziusudra.Rencode.Tests
{
    public class Int64EncoderTests
    {

        [Theory]
        [InlineData(2147483648, new byte[] { 0x41, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 })]
        [InlineData(32768, new byte[] { 0x40, 0x00, 0x00, 0x80, 0x00 })]
        [InlineData(-32769, new byte[] { 0x40, 0xFF, 0xFF, 0x7F, 0xFF })]
        [InlineData(128, new byte[] { 0x3F, 0x00, 0x80 })]
        [InlineData(-129, new byte[] { 0x3F, 0xFF, 0x7F })]
        [InlineData(1, new byte[] { 0x01 })]
        [InlineData(-5, new byte[] { 0x4A })]
        [InlineData(46, new byte[] { 0x3E, 0x2E })]
        [InlineData(-47, new byte[] { 0x3E, 0xD1 })]
        public async void WriteAsync_ShouldProperlyEncodeAValue(long value, byte[] expected)
        {
            Int64Encoder encoder = new();
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, value, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
