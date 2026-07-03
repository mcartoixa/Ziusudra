using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ziusudra.Client.Tests
{

    public class ConnectionManagerTests
    {

        [Fact]
        public async Task LoadAsync_ShouldPopulateHostsFromTheStore()
        {
            var store = new InMemoryHostStore(new HostEntry { Host = "a" }, new HostEntry { Host = "b" });
            ConnectionManager manager = NewManager(store: store);

            await manager.LoadAsync();

            Assert.Equal(2, manager.Hosts.Count);
        }

        [Fact]
        public async Task AddAsync_ShouldAppendAndPersist()
        {
            var store = new InMemoryHostStore();
            ConnectionManager manager = NewManager(store: store);

            await manager.AddAsync(new HostEntry { Host = "nas.local" });

            Assert.Equal("nas.local", Assert.Single(manager.Hosts).Host);
            Assert.Equal("nas.local", Assert.Single(store.Saved).Host);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveByIdAndPersist()
        {
            var keep = new HostEntry { Host = "keep" };
            var drop = new HostEntry { Host = "drop" };
            var store = new InMemoryHostStore(keep, drop);
            ConnectionManager manager = NewManager(store: store);
            await manager.LoadAsync();

            await manager.RemoveAsync(drop);

            Assert.Equal("keep", Assert.Single(manager.Hosts).Host);
            Assert.Equal("keep", Assert.Single(store.Saved).Host);
        }

        [Fact]
        public async Task ConnectAsync_ShouldResolveThenReachConnected()
        {
            var resolver = new FakeHostResolver();
            ConnectionManager manager = NewManager(resolver: resolver);

            await manager.ConnectAsync(new HostEntry { Host = "nas.local", Port = 58846, Username = "admin" }, "secret");

            Assert.Equal(SessionState.Connected, manager.State);
            Assert.Equal("nas.local", resolver.LastHost);
            Assert.Equal(58846, resolver.LastPort);
            Assert.Equal("2.0.0", manager.Capabilities!.DaemonVersion);
        }

        [Fact]
        public async Task StateChanged_ShouldForwardSessionTransitions()
        {
            ConnectionManager manager = NewManager();
            int changes = 0;
            manager.StateChanged += (_, _) => changes++;

            await manager.ConnectAsync(new HostEntry { Host = "nas.local" }, "secret");

            Assert.True(changes >= 1);
            Assert.Equal(SessionState.Connected, manager.State);
        }

        private static ConnectionManager NewManager(IHostStore? store = null, IHostResolver? resolver = null)
        {
            var client = new FakeRpcClient(new IPEndPoint(IPAddress.Loopback, HostEntry.DefaultPort))
                .Respond("daemon.info", "2.0.0")
                .Respond("daemon.login", 5)
                .Respond("daemon.set_event_interest", true)
                .Respond("daemon.get_method_list", new[] { "core.get_torrents_status" });
            var session = new DelugeSession(_ => client);
            return new ConnectionManager(session, store ?? new InMemoryHostStore(), resolver ?? new FakeHostResolver());
        }
    }

    internal sealed class InMemoryHostStore:
        IHostStore
    {

        public InMemoryHostStore(params HostEntry[] entries)
        {
            Saved = new List<HostEntry>(entries);
        }

        public Task<IReadOnlyList<HostEntry>> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<HostEntry>>(Saved.ToList());
        }

        public Task SaveAsync(IEnumerable<HostEntry> entries, CancellationToken cancellationToken = default)
        {
            Saved = entries.ToList();
            return Task.CompletedTask;
        }

        public List<HostEntry> Saved { get; private set; }
    }

    internal sealed class FakeHostResolver:
        IHostResolver
    {

        public Task<IPEndPoint> ResolveAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            LastHost = host;
            LastPort = port;
            return Task.FromResult(new IPEndPoint(IPAddress.Loopback, port));
        }

        public string? LastHost { get; private set; }

        public int LastPort { get; private set; }
    }
}
