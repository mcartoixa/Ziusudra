using System.Collections;
using Xunit;

namespace Ziusudra.DelugeRpc.Tests
{
    public class MessageExtensionsTests
    {

        public class FakeMessage:
            IMessage
        {

            public FakeMessage(object? value)
            {
                var values = new ArrayList();
                values.Add(value);
                Values = values;
            }

            public FakeMessage(ICollection values)
            {
                Values = values;
            }

            public ICollection ToValueCollection()
            {
                return Values;
            }

            public ICollection Values { get; }
        }

        [Theory]
        [InlineData(1, "[1]")]
        [InlineData(1.2f, "[1.2]")]
        [InlineData(null, "[<null>]")]
        [InlineData("test", "[\"test\"]")]
        public void ToMessageString_ProperlySerializesCommonTypes(object? value, string expected)
        {
            IMessage message = new FakeMessage(value);

            string actual = message.ToDebugString();

            Assert.Equal(expected, actual);
        }
    }
}
