using System.Threading;
using Xunit;

namespace Ziusudra.Rencode.Tests
{
    public class NullEncoderTests
    {

        [Fact]
        public async void WriteAsync_ShouldProperlyEncodeAValue()
        {
            NullEncoder encoder = new();
            byte[] expected = new byte[] { 0x45 };
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, null, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
