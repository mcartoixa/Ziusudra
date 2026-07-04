namespace Ziusudra.Client
{

    /// <summary>The torrent status fields the client requests from the daemon.</summary>
    public static class TorrentStatusFields
    {

        /// <summary>Gets the default set of status fields the <see cref="TorrentMonitor" /> projects.</summary>
        /// <remarks>Mirrors the fields <see cref="Ziusudra.DelugeRpc.Core.Torrent" /> understands. A later step lets
        /// the visible columns drive this set (as the GTK client only fetches visible columns).</remarks>
        public static IReadOnlyList<string> Default { get; } = new[] {
            "active_time",
            "download_payload_rate",
            "eta",
            "hash",
            "name",
            "progress",
            "queue",
            "seeding_time",
            "state",
            "total_wanted",
            "upload_payload_rate"
        };
    }
}
