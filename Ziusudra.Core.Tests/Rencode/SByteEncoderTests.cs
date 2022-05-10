using System.Threading;
using Xunit;

namespace Ziusudra.Rencode.Tests
{
    public class SByteEncoderTests
    {

        [Theory]
        [InlineData(1, new byte[] { 0x01 })]
        [InlineData(-5, new byte[] { 0x4A })]
        [InlineData(46, new byte[] { 0x3E, 0x2E })]
        [InlineData(-47, new byte[] { 0x3E, 0xD1 })]
        public async void WriteAsync_ShouldProperlyEncodeAValue(sbyte value, byte[] expected)
        {
            SByteEncoder encoder = new();
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, value, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
