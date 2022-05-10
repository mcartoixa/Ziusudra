using System.Threading;
using Xunit;

namespace Ziusudra.Rencode.Tests
{
    public class Int16EncoderTests
    {

        [Theory]
        [InlineData(128, new byte[] { 0x3F, 0x00, 0x80 })]
        [InlineData(-129, new byte[] { 0x3F, 0xFF, 0x7F })]
        [InlineData(1, new byte[] { 0x01 })]
        [InlineData(-5, new byte[] { 0x4A })]
        [InlineData(46, new byte[] { 0x3E, 0x2E })]
        [InlineData(-47, new byte[] { 0x3E, 0xD1 })]
        public async void WriteAsync_ShouldProperlyEncodeAValue(short value, byte[] expected)
        {
            Int16Encoder encoder = new();
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, value, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
