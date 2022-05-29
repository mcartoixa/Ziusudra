using System.Collections;
using AutoFixture;
using Xunit;

namespace Ziusudra.DelugeRpc.Tests
{

    public class RpcServerExceptionTests
    {

        [Fact]
        public void Constructor_ShouldInitializeRemoteStackTrace()
        {
            Fixture fixture = new();
            ArrayList values = new();
            values.Add((int)RpcMessageType.RPC_ERROR);
            values.Add(fixture.Create<int>()); // request_id
            values.Add(fixture.Create<string>()); // exception_type
            values.Add(fixture.Create<string>()); // exception_msg
            values.Add(fixture.Create<string>()); // traceback

            RpcServerException exception = new(values);

            Assert.Equal((int)(values[1] ?? 0), ((IExchangeMessage)exception).Id);
            Assert.Equal((string)(values[2] ?? string.Empty), exception.Source);
            Assert.Equal((string)(values[3] ?? string.Empty), exception.Message);
            Assert.Equal((string)(values[4] ?? string.Empty), exception.StackTrace);
        }
    }
}
