using Xunit;

namespace Ziusudra.Client.Tests
{

    public class ServerCapabilitiesTests
    {

        [Fact]
        public void HasMethod_ShouldMatchExactlyAndCaseSensitively()
        {
            var capabilities = new ServerCapabilities("2.0.0", "2.0.3", new[] { "core.pause_torrent" });

            Assert.True(capabilities.HasMethod("core.pause_torrent"));
            Assert.False(capabilities.HasMethod("core.Pause_Torrent"));
            Assert.False(capabilities.HasMethod("core.resume_torrent"));
        }

        [Fact]
        public void Methods_ShouldDeduplicateTheProvidedNames()
        {
            var capabilities = new ServerCapabilities("2.0.0", string.Empty, new[] { "a", "b", "a" });

            Assert.Equal(2, capabilities.Methods.Count);
        }
    }
}
