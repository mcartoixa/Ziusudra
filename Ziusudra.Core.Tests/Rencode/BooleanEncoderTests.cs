using System.Threading;
using Xunit;

namespace Ziusudra.Rencode.Tests
{
    public class BooleanEncoderTests
    {

        [Theory]
        [InlineData(true, new byte[] { 0x43 })]
        [InlineData(false, new byte[] { 0x44 })]
        public async void WriteAsync_ShouldProperlyEncodeAValue(bool value, byte[] expected)
        {
            BooleanEncoder encoder = new();
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, value, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
