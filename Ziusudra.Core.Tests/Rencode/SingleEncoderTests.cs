using System.Threading;
using Xunit;

namespace Ziusudra.Rencode.Tests
{

    public class SingleEncoderTests
    {

        [Theory]
        [InlineData(1.125, new byte[] { 0x42, 0x3F, 0x90, 0x00, 0x00 })]
        public async void WriteAsync_ShouldProperlyEncodeAValue(float value, byte[] expected)
        {
            SingleEncoder encoder = new();
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, value, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
