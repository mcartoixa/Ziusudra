using System.Collections;
using System.Diagnostics;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Represents a torrent.</summary>
    public class Torrent
    {

        internal Torrent(string identifier, IDictionary values)
        {
            ActiveTime = TimeSpan.FromSeconds(Convert.ToInt32(values["active_time"]));
            Hash = values["hash"]?.ToString() ?? string.Empty;
            Debug.Assert(string.Equals(Hash, identifier, StringComparison.Ordinal));
            Name = values["name"]?.ToString() ?? string.Empty;
            Progress = Convert.ToSingle(values["progress"]) / 100;
            SeedingTime = TimeSpan.FromSeconds(Convert.ToInt32(values["seeding_time"]));
        }

        /// <summary>Gets or sets the active time for the torrent.</summary>
        public TimeSpan ActiveTime { get; set; }
        /// <summary>Gets the hash of the torrent.</summary>
        public string Hash { get; private set; }
        /// <summary>Gets or sets the name of the torrent.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the progress of the torrent.</summary>
        public float Progress { get; set; }
        /// <summary>Gets or sets the seeding time of the torrent.</summary>
        public TimeSpan SeedingTime { get; set; }
    }
}
