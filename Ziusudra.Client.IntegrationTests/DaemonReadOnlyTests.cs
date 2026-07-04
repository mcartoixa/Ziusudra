using System.Threading.Tasks;
using Xunit;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Client.IntegrationTests
{

    /// <summary>Read-only checks against a real Deluge daemon, exercising the wire path the unit tests cannot cover.</summary>
    /// <remarks>These leave no trace on the server. Mutating flows (add / pause / remove) wait for the command facade
    /// in a later step, behind an explicit opt-in with cleanup.</remarks>
    public class DaemonReadOnlyTests
    {

        [SkippableFact]
        public async Task Connect_ShouldReachConnectedAndReportADaemonVersion()
        {
            Skip.IfNot(DelugeTestEnvironment.IsConfigured, DelugeTestEnvironment.SkipReason);

            await using DelugeSession session = await DelugeTestEnvironment.ConnectAsync();

            Assert.Equal(SessionState.Connected, session.State);
            Assert.False(string.IsNullOrWhiteSpace(session.Capabilities!.DaemonVersion));
        }

        [SkippableFact]
        public async Task Capabilities_ShouldAdvertiseTheTorrentStatusMethod()
        {
            Skip.IfNot(DelugeTestEnvironment.IsConfigured, DelugeTestEnvironment.SkipReason);

            await using DelugeSession session = await DelugeTestEnvironment.ConnectAsync();

            Assert.True(session.Capabilities!.HasMethod("core.get_torrents_status"));
        }

        [SkippableFact]
        public async Task TorrentMonitor_ShouldDecodeTorrentStatusFromTheDaemon()
        {
            Skip.IfNot(DelugeTestEnvironment.IsConfigured, DelugeTestEnvironment.SkipReason);

            await using DelugeSession session = await DelugeTestEnvironment.ConnectAsync();
            await session.Torrents!.RefreshAsync();

            foreach (Torrent torrent in session.Torrents.Torrents)
                Assert.False(string.IsNullOrEmpty(torrent.Hash));
        }
    }
}
