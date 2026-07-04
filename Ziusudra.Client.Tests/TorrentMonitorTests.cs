using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Ziusudra.DelugeRpc;
using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Client.Tests
{

    public class TorrentMonitorTests
    {

        [Fact]
        public async Task RefreshAsync_ShouldAddNewTorrentsAndPopulateTheModel()
        {
            FakeRpcClient client = ClientReturning(
                Torrents(("h1", Fields(("name", "Alpha"), ("state", "Downloading")))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            var added = new List<Torrent>();
            monitor.TorrentAdded += (_, e) => added.Add(e.Torrent);

            await monitor.RefreshAsync();

            Assert.Equal("Alpha", Assert.Single(added).Name);
            Assert.Equal("h1", Assert.Single(monitor.Torrents).Hash);
        }

        [Fact]
        public async Task RefreshAsync_ShouldRaiseTorrentUpdatedWhenAFieldChanges()
        {
            FakeRpcClient client = ClientReturning(
                Torrents(("h1", Fields(("name", "Alpha"), ("download_payload_rate", 100)))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            await monitor.RefreshAsync();
            var updated = new List<Torrent>();
            monitor.TorrentUpdated += (_, e) => updated.Add(e.Torrent);

            client.Respond("core.get_torrents_status",
                Torrents(("h1", Fields(("name", "Alpha"), ("download_payload_rate", 250)))));
            await monitor.RefreshAsync();

            Assert.Equal(250, Assert.Single(updated).DownloadPayloadRate);
        }

        [Fact]
        public async Task RefreshAsync_ShouldNotRaiseTorrentUpdatedWhenNothingChanged()
        {
            FakeRpcClient client = ClientReturning(
                Torrents(("h1", Fields(("name", "Alpha"), ("download_payload_rate", 100)))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            await monitor.RefreshAsync();
            int updates = 0;
            monitor.TorrentUpdated += (_, _) => updates++;

            await monitor.RefreshAsync();

            Assert.Equal(0, updates);
        }

        [Fact]
        public async Task RefreshAsync_ShouldCompareByValueNotByType()
        {
            // rencode round-trips to the most compact CLR type, so the same value can arrive as int then sbyte.
            FakeRpcClient client = ClientReturning(
                Torrents(("h1", Fields(("name", "Alpha"), ("download_payload_rate", 100)))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            await monitor.RefreshAsync();
            int updates = 0;
            monitor.TorrentUpdated += (_, _) => updates++;

            client.Respond("core.get_torrents_status",
                Torrents(("h1", Fields(("name", "Alpha"), ("download_payload_rate", (sbyte)100)))));
            await monitor.RefreshAsync();

            Assert.Equal(0, updates);
        }

        [Fact]
        public async Task RefreshAsync_ShouldRaiseTorrentRemovedWhenATorrentDisappears()
        {
            FakeRpcClient client = ClientReturning(
                Torrents(
                    ("h1", Fields(("name", "Alpha"))),
                    ("h2", Fields(("name", "Beta")))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            await monitor.RefreshAsync();
            var removed = new List<Torrent>();
            monitor.TorrentRemoved += (_, e) => removed.Add(e.Torrent);

            client.Respond("core.get_torrents_status", Torrents(("h1", Fields(("name", "Alpha")))));
            await monitor.RefreshAsync();

            Assert.Equal("h2", Assert.Single(removed).Hash);
            Assert.Equal("h1", Assert.Single(monitor.Torrents).Hash);
        }

        [Fact]
        public async Task TorrentRemovedEvent_ShouldRemoveFromTheModelImmediately()
        {
            FakeRpcClient client = ClientReturning(
                Torrents(
                    ("h1", Fields(("name", "Alpha"))),
                    ("h2", Fields(("name", "Beta")))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            await monitor.RefreshAsync();
            var removed = new List<Torrent>();
            monitor.TorrentRemoved += (_, e) => removed.Add(e.Torrent);

            client.Raise(Event("TorrentRemovedEvent", "h1"));

            Assert.Equal("h1", Assert.Single(removed).Hash);
            Assert.Equal("h2", Assert.Single(monitor.Torrents).Hash);
        }

        [Fact]
        public async Task TorrentAddedEvent_ShouldTriggerARefresh()
        {
            FakeRpcClient client = ClientReturning(Torrents(("h1", Fields(("name", "Alpha")))));
            await using var monitor = new TorrentMonitor(client, TorrentStatusFields.Default);
            var added = new List<Torrent>();
            monitor.TorrentAdded += (_, e) => added.Add(e.Torrent);
            var refreshed = new TaskCompletionSource();
            monitor.Refreshed += (_, _) => refreshed.TrySetResult();

            client.Raise(Event("TorrentAddedEvent", "h1", true));
            await refreshed.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.Equal("h1", Assert.Single(added).Hash);
        }

        private static FakeRpcClient ClientReturning(Hashtable torrents)
        {
            return new FakeRpcClient(new IPEndPoint(IPAddress.Loopback, 58846))
                .Respond("core.get_torrents_status", torrents);
        }

        private static Hashtable Torrents(params (string Id, Hashtable Fields)[] entries)
        {
            var torrents = new Hashtable();
            foreach ((string id, Hashtable fields) in entries)
                torrents[id] = fields;
            return torrents;
        }

        private static Hashtable Fields(params (string Key, object Value)[] pairs)
        {
            var fields = new Hashtable();
            foreach ((string key, object value) in pairs)
                fields[key] = value;
            return fields;
        }

        private static RpcEvent Event(string name, params object[] data)
        {
            var values = new ArrayList { (int)RpcMessageType.RPC_EVENT, name, new ArrayList(data) };
            return RpcEvent.CreateFromValues(values);
        }
    }
}
