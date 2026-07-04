using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Client.IntegrationTests
{

    /// <summary>Mutating checks against a real Deluge daemon, exercising the command facade end to end.</summary>
    /// <remarks>Each test operates on its own throwaway torrent (a random infohash, so reruns never collide) and
    /// removes it as its final asserted step, leaving the daemon as it was found. Commands apply server-side
    /// asynchronously, so the assertions poll the daemon for the expected status rather than reading it once.</remarks>
    public class DaemonCommandTests
    {

        [SkippableFact]
        public async Task AddMagnet_PauseResumeRemove_ShouldReflectOnTheDaemon()
        {
            Skip.IfNot(DelugeTestEnvironment.IsConfigured, DelugeTestEnvironment.SkipReason);

            await using DelugeSession session = await DelugeTestEnvironment.ConnectAsync();

            string magnet = NewMagnet(out string hash);
            string? id = await session.AddMagnetAsync(magnet);
            Assert.Equal(hash, id, ignoreCase: true);

            Torrent? added = await WaitAsync(session, hash, torrent => torrent is not null);
            Assert.NotNull(added);

            await session.PauseAsync(new[] { hash });
            Torrent? paused = await WaitAsync(session, hash, torrent => torrent?.State == TorrentState.Paused);
            Assert.Equal(TorrentState.Paused, paused?.State);

            await session.ResumeAsync(new[] { hash });
            Torrent? resumed = await WaitAsync(session, hash, torrent => torrent is not null && torrent.State != TorrentState.Paused);
            Assert.NotNull(resumed);
            Assert.NotEqual(TorrentState.Paused, resumed!.State);

            bool removed = await session.RemoveAsync(hash, removeData: true);
            Assert.True(removed);

            Torrent? gone = await WaitAsync(session, hash, torrent => torrent is null);
            Assert.Null(gone);
        }

        [SkippableFact]
        public async Task AddTorrentFile_ShouldAppearThenBeRemoved()
        {
            Skip.IfNot(DelugeTestEnvironment.IsConfigured, DelugeTestEnvironment.SkipReason);

            await using DelugeSession session = await DelugeTestEnvironment.ConnectAsync();

            byte[] content = BuildMinimalTorrent(out string name);
            string? id = await session.AddTorrentFileAsync(name, content);
            Assert.False(string.IsNullOrEmpty(id));

            Torrent? added = await WaitAsync(session, id!, torrent => torrent is not null);
            Assert.NotNull(added);

            bool removed = await session.RemoveAsync(id!, removeData: true);
            Assert.True(removed);

            Torrent? gone = await WaitAsync(session, id!, torrent => torrent is null);
            Assert.Null(gone);
        }

        private static async Task<Torrent?> WaitAsync(DelugeSession session, string hash, Func<Torrent?, bool> matched)
        {
            Torrent? torrent = null;
            for (int attempt = 0; attempt < PollAttempts; attempt++)
            {
                await session.Torrents!.RefreshAsync();
                torrent = session.Torrents.Torrents
                    .FirstOrDefault(t => string.Equals(t.Hash, hash, StringComparison.OrdinalIgnoreCase));
                if (matched(torrent))
                    return torrent;
                await Task.Delay(PollInterval);
            }
            return torrent;
        }

        private static string NewMagnet(out string infoHash)
        {
            infoHash = Convert.ToHexString(RandomNumberGenerator.GetBytes(20)).ToLowerInvariant();
            return $"magnet:?xt=urn:btih:{infoHash}&dn=ziusudra-integration-test";
        }

        private static byte[] BuildMinimalTorrent(out string name)
        {
            name = "ziusudra-integration-test.bin";
            byte[] pieces = RandomNumberGenerator.GetBytes(20);

            // A trackerless single-file metainfo. Keys within each dictionary are in the ascending order bencode
            // requires; libtorrent parses this on add without validating the piece hash against any data.
            using var buffer = new MemoryStream();
            void Write(string ascii) => buffer.Write(Encoding.ASCII.GetBytes(ascii));
            Write("d4:infod");
            Write("6:lengthi16384e");
            Write($"4:name{name.Length}:{name}");
            Write("12:piece lengthi16384e");
            Write("6:pieces20:");
            buffer.Write(pieces);
            Write("ee");
            return buffer.ToArray();
        }

        private const int PollAttempts = 40;
        private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);
    }
}
