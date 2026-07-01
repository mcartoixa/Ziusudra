using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ziusudra.Rencode.Tests
{

    public class ListEncoderTests
    {

        [Theory]
        [InlineData(new object?[] { null, null, null }, new byte[] { 0xC3, FakeMemoryStreamWriter.DEFAULT_VALUE, FakeMemoryStreamWriter.DEFAULT_VALUE, FakeMemoryStreamWriter.DEFAULT_VALUE })]
        public async Task WriteAsync_ShouldProperlyEncodeAValue(ICollection value, byte[] expected)
        {
            ListEncoder encoder = new();
            FakeMemoryStreamWriter writer = new();

            await encoder.WriteValueAsync(writer, value, CancellationToken.None);

            Assert.Equal(expected, writer.Buffer);
        }
    }
}
