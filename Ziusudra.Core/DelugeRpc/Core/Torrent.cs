using System.Collections;
using System.Diagnostics;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Represents a torrent.</summary>
    public class Torrent
    {

        internal Torrent(string identifier, IDictionary values)
        {
            if (values.Contains("hash"))
            {
                Hash = values["hash"]?.ToString() ?? identifier;
                Debug.Assert(string.Equals(Hash, identifier, StringComparison.Ordinal));
            } else
                Hash = identifier;

            if (values.Contains("active_time"))
                ActiveTime = TimeSpan.FromSeconds(Convert.ToInt32(values["active_time"]));
            if (values.Contains("download_payload_rate"))
                DownloadPayloadRate = Convert.ToInt32(values["download_payload_rate"]);
            if (values.Contains("eta"))
                ExpectedTimeOfArrival = TimeSpan.FromSeconds(Convert.ToInt32(values["eta"]));
            if (values.Contains("name"))
                Name = values["name"]?.ToString() ?? string.Empty;
            if (values.Contains("progress"))
                Progress = Convert.ToSingle(values["progress"]) / 100;
            if (values.Contains("queue"))
            {
                int q = Convert.ToInt32(values["queue"]);
                Queue = q >= 0 ? q + 1 : null;
            }
            if (values.Contains("state"))
            {
                TorrentState s;
                if (Enum.TryParse(values["state"]?.ToString(), out s))
                    State = s;
                else
                    State = null;
            }
            if (values.Contains("seeding_time"))
                SeedingTime = TimeSpan.FromSeconds(Convert.ToInt32(values["seeding_time"]));
            if (values.Contains("total_wanted"))
                TotalWanted = Convert.ToInt64(values["total_wanted"]);
            if (values.Contains("upload_payload_rate"))
                UploadPayloadRate = Convert.ToInt32(values["upload_payload_rate"]);
        }

        /// <summary>Gets or sets the active time for the torrent.</summary>
        public TimeSpan? ActiveTime { get; set; }
        /// <summary>Gets or sets the download payload rate of the torrent.</summary>
        public int? DownloadPayloadRate { get; set; }
        /// <summary>Gets or sets the expected time of arrival of the torrent.</summary>
        public TimeSpan? ExpectedTimeOfArrival { get; set; }
        /// <summary>Gets the hash of the torrent.</summary>
        public string Hash { get; private set; }
        /// <summary>Gets or sets the name of the torrent.</summary>
        public string? Name { get; set; }
        /// <summary>Gets or sets the progress of the torrent.</summary>
        public float? Progress { get; set; }
        /// <summary>Gets or sets the queue number of the torrent.</summary>
        public int? Queue { get; set; }
        /// <summary>Gets or sets the seeding time of the torrent.</summary>
        public TimeSpan? SeedingTime { get; set; }
        /// <summary>Gets or sets the state of the torrent.</summary>
        public TorrentState? State { get; set; }
        /// <summary>Gets or sets the total size of the torrent.</summary>
        public long? TotalWanted { get; set; }
        /// <summary>Gets or sets the upload payload rate of the torrent.</summary>
        public int? UploadPayloadRate { get; set; }
    }
}
