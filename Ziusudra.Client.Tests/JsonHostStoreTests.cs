using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Ziusudra.Client.Tests
{

    public class JsonHostStoreTests:
        IDisposable
    {

        public JsonHostStoreTests()
        {
            _Path = Path.Combine(Path.GetTempPath(), $"ziusudra-hosts-{Guid.NewGuid():N}", "hosts.json");
        }

        [Fact]
        public async Task LoadAsync_ShouldReturnEmptyWhenTheFileDoesNotExist()
        {
            var store = new JsonHostStore(_Path);

            IReadOnlyList<HostEntry> entries = await store.LoadAsync();

            Assert.Empty(entries);
        }

        [Fact]
        public async Task SaveAsync_ThenLoadAsync_ShouldRoundTripEntries()
        {
            var store = new JsonHostStore(_Path);
            var saved = new[] {
                new HostEntry { Name = "home", Host = "nas.local", Port = 58846, Username = "admin" },
                new HostEntry { Name = "seedbox", Host = "seed.example.com", Port = 12345, Username = "me" }
            };

            await store.SaveAsync(saved);
            IReadOnlyList<HostEntry> loaded = await store.LoadAsync();

            Assert.Equal(2, loaded.Count);
            Assert.Equal(saved[0].Id, loaded[0].Id);
            Assert.Equal("seed.example.com", loaded[1].Host);
            Assert.Equal(12345, loaded[1].Port);
            Assert.Equal("me", loaded[1].Username);
        }

        [Fact]
        public async Task SaveAsync_ShouldReplaceThePreviousList()
        {
            var store = new JsonHostStore(_Path);
            await store.SaveAsync(new[] { new HostEntry { Host = "first" } });

            await store.SaveAsync(new[] { new HostEntry { Host = "second" } });
            IReadOnlyList<HostEntry> loaded = await store.LoadAsync();

            Assert.Equal("second", Assert.Single(loaded).Host);
        }

        public void Dispose()
        {
            string? directory = Path.GetDirectoryName(_Path);
            if (directory != null && Directory.Exists(directory))
                Directory.Delete(directory, true);
            GC.SuppressFinalize(this);
        }

        private readonly string _Path;
    }
}
