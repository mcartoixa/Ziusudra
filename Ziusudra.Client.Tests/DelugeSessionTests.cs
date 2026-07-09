using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Client.Tests
{

    public class DelugeSessionTests
    {

        [Fact]
        public async Task ConnectAsync_ShouldDriveTheBootstrapSequenceInOrder()
        {
            FakeRpcClient client = FullyScriptedClient();
            await using var session = new DelugeSession(_ => client);

            await session.ConnectAsync(Endpoint, "user", "pass");

            Assert.Equal(SessionState.Connected, session.State);
            Assert.True(client.IsStarted);
            Assert.Equal(
                new[] { "daemon.info", "daemon.login", "daemon.set_event_interest", "daemon.get_method_list", "core.get_libtorrent_version" },
                client.SentMethods);
        }

        [Fact]
        public async Task ConnectAsync_ShouldExposeServerCapabilities()
        {
            FakeRpcClient client = FullyScriptedClient();
            await using var session = new DelugeSession(_ => client);

            await session.ConnectAsync(Endpoint, "user", "pass");

            Assert.NotNull(session.Capabilities);
            Assert.Equal("2.0.0", session.Capabilities!.DaemonVersion);
            Assert.Equal("2.0.3", session.Capabilities.LibtorrentVersion);
            Assert.True(session.Capabilities.HasMethod("core.get_torrents_status"));
            Assert.False(session.Capabilities.HasMethod("core.does_not_exist"));
            Assert.Equal(5, session.AuthenticationLevel);
        }

        [Fact]
        public async Task ConnectAsync_ShouldSkipLibtorrentVersionWhenTheMethodIsAbsent()
        {
            FakeRpcClient client = new FakeRpcClient(Endpoint)
                .Respond("daemon.info", "2.0.0")
                .Respond("daemon.login", 5)
                .Respond("daemon.set_event_interest", true)
                .Respond("daemon.get_method_list", new[] { "core.get_torrents_status" });
            await using var session = new DelugeSession(_ => client);

            await session.ConnectAsync(Endpoint, "user", "pass");

            Assert.DoesNotContain("core.get_libtorrent_version", client.SentMethods);
            Assert.Equal(string.Empty, session.Capabilities!.LibtorrentVersion);
        }

        [Fact]
        public async Task ConnectAsync_ShouldTransitionThroughConnectingAuthenticatingConnected()
        {
            FakeRpcClient client = FullyScriptedClient();
            await using var session = new DelugeSession(_ => client);
            var states = new List<SessionState>();
            session.StateChanged += (_, _) => states.Add(session.State);

            await session.ConnectAsync(Endpoint, "user", "pass");

            Assert.Equal(
                new[] { SessionState.Connecting, SessionState.Authenticating, SessionState.Connected },
                states);
        }

        [Fact]
        public async Task ConnectAsync_ShouldFaultAndStopTheClientWhenLoginFails()
        {
            FakeRpcClient client = new FakeRpcClient(Endpoint)
                .Respond("daemon.info", "2.0.0")
                .Fail("daemon.login", new InvalidOperationException("bad login"));
            await using var session = new DelugeSession(_ => client);

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await session.ConnectAsync(Endpoint, "user", "pass"));

            Assert.Equal(SessionState.Faulted, session.State);
            Assert.True(client.IsStopped);
            Assert.Null(session.Capabilities);
        }

        [Fact]
        public async Task ConnectAsync_ShouldRejectWhenAlreadyConnected()
        {
            FakeRpcClient client = FullyScriptedClient();
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await session.ConnectAsync(Endpoint, "user", "pass"));
        }

        [Fact]
        public async Task ConnectAsync_ShouldRecoverFromAFaultedState()
        {
            FakeRpcClient failing = new FakeRpcClient(Endpoint)
                .Respond("daemon.info", "2.0.0")
                .Fail("daemon.login", new InvalidOperationException("bad login"));
            FakeRpcClient succeeding = FullyScriptedClient();
            var clients = new Queue<FakeRpcClient>(new[] { failing, succeeding });
            await using var session = new DelugeSession(_ => clients.Dequeue());

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await session.ConnectAsync(Endpoint, "user", "pass"));
            await session.ConnectAsync(Endpoint, "user", "pass");

            Assert.Equal(SessionState.Connected, session.State);
        }

        [Fact]
        public async Task DisconnectAsync_ShouldStopTheClientAndResetState()
        {
            FakeRpcClient client = FullyScriptedClient();
            var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            await session.DisconnectAsync();

            Assert.Equal(SessionState.Disconnected, session.State);
            Assert.True(client.IsStopped);
            Assert.Null(session.Capabilities);
        }

        [Fact]
        public async Task PauseAsync_ShouldSendPauseTorrentsWhenSupported()
        {
            FakeRpcClient client = CommandClient("core.pause_torrents", returnValue: null);
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            await session.PauseAsync(new[] { "h1" });

            Assert.Contains("core.pause_torrents", client.SentMethods);
        }

        [Fact]
        public async Task ResumeAsync_ShouldSendResumeTorrentsWhenSupported()
        {
            FakeRpcClient client = CommandClient("core.resume_torrents", returnValue: null);
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            await session.ResumeAsync(new[] { "h1" });

            Assert.Contains("core.resume_torrents", client.SentMethods);
        }

        [Fact]
        public async Task RemoveAsync_ShouldSendRemoveTorrentAndReturnTheResult()
        {
            FakeRpcClient client = CommandClient("core.remove_torrent", returnValue: true);
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            bool removed = await session.RemoveAsync("h1", removeData: true);

            Assert.True(removed);
            Assert.Contains("core.remove_torrent", client.SentMethods);
        }

        [Fact]
        public async Task RemoveAsync_ShouldSendRemoveTorrentsAndReportNoErrorsOnSuccess()
        {
            FakeRpcClient client = CommandClient("core.remove_torrents", returnValue: Array.Empty<object>());
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            IReadOnlyList<RemoveTorrentError> errors = await session.RemoveAsync(new[] { "h1", "h2" }, removeData: true);

            Assert.Empty(errors);
            Assert.Contains("core.remove_torrents", client.SentMethods);
        }

        [Fact]
        public async Task RemoveAsync_ShouldReturnTheReportedRemovalErrors()
        {
            FakeRpcClient client = CommandClient("core.remove_torrents", returnValue: new object[] { new object[] { "h2", "Torrent not found" } });
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            IReadOnlyList<RemoveTorrentError> errors = await session.RemoveAsync(new[] { "h1", "h2" }, removeData: false);

            RemoveTorrentError error = Assert.Single(errors);
            Assert.Equal("h2", error.TorrentId);
            Assert.Equal("Torrent not found", error.Message);
        }

        [Fact]
        public async Task AddMagnetAsync_ShouldSendAddTorrentMagnetAndReturnTheId()
        {
            FakeRpcClient client = CommandClient("core.add_torrent_magnet", returnValue: "newhash");
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            string? id = await session.AddMagnetAsync("magnet:?xt=urn:btih:abc");

            Assert.Equal("newhash", id);
            Assert.Contains("core.add_torrent_magnet", client.SentMethods);
        }

        [Fact]
        public async Task AddTorrentFileAsync_ShouldSendAddTorrentFileAndReturnTheId()
        {
            FakeRpcClient client = CommandClient("core.add_torrent_file", returnValue: "newhash");
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            string? id = await session.AddTorrentFileAsync("a.torrent", new byte[] { 1, 2, 3 });

            Assert.Equal("newhash", id);
            Assert.Contains("core.add_torrent_file", client.SentMethods);
        }

        [Fact]
        public async Task GetFilterTreeAsync_ShouldReturnTheCategoriesWithTheirValuesAndCounts()
        {
            var tree = new Hashtable {
                ["state"] = new ArrayList {
                    new ArrayList { "All", 3 },
                    new ArrayList { "Downloading", 1 },
                }
            };
            FakeRpcClient client = CommandClient("core.get_filter_tree", returnValue: tree);
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            IReadOnlyDictionary<string, IReadOnlyList<Filter>> result = await session.GetFilterTreeAsync();

            IReadOnlyList<Filter> states = result["state"];
            Assert.Equal(2, states.Count);
            Assert.Equal(3, states.Single(f => f.Value == "All").Count);
            Assert.Equal(1, states.Single(f => f.Value == "Downloading").Count);
            Assert.Contains("core.get_filter_tree", client.SentMethods);
        }

        [Fact]
        public async Task Command_ShouldThrowNotSupportedWhenTheMethodIsAbsent()
        {
            FakeRpcClient client = FullyScriptedClient();
            await using var session = new DelugeSession(_ => client);
            await session.ConnectAsync(Endpoint, "user", "pass");

            await Assert.ThrowsAsync<NotSupportedException>(
                async () => await session.PauseAsync(new[] { "h1" }));
        }

        [Fact]
        public async Task Command_ShouldThrowWhenNotConnected()
        {
            await using var session = new DelugeSession(_ => FullyScriptedClient());

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await session.PauseAsync(new[] { "h1" }));
        }

        private static FakeRpcClient CommandClient(string method, object? returnValue)
        {
            return new FakeRpcClient(Endpoint)
                .Respond("daemon.info", "2.0.0")
                .Respond("daemon.login", 5)
                .Respond("daemon.set_event_interest", true)
                .Respond("daemon.get_method_list", new[] { method })
                .Respond(method, returnValue);
        }

        private static FakeRpcClient FullyScriptedClient()
        {
            return new FakeRpcClient(Endpoint)
                .Respond("daemon.info", "2.0.0")
                .Respond("daemon.login", 5)
                .Respond("daemon.set_event_interest", true)
                .Respond("daemon.get_method_list", new[] { "core.get_libtorrent_version", "core.get_torrents_status" })
                .Respond("core.get_libtorrent_version", "2.0.3");
        }

        private static IPEndPoint Endpoint => new(IPAddress.Loopback, 58846);
    }
}
