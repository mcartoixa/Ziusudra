namespace Ziusudra.Client
{

    /// <summary>Carries the probed status of a single host.</summary>
    public sealed class HostStatusChangedEventArgs:
        EventArgs
    {

        /// <summary>Create a new instance of the <see cref="HostStatusChangedEventArgs" /> type.</summary>
        /// <param name="hostId">The identifier of the host the status belongs to.</param>
        /// <param name="status">The probed status.</param>
        /// <param name="version">The daemon version reported by the probe, or empty when unknown.</param>
        public HostStatusChangedEventArgs(Guid hostId, HostStatus status, string version)
        {
            HostId = hostId;
            Status = status;
            Version = version;
        }

        /// <summary>Gets the identifier of the host the status belongs to.</summary>
        public Guid HostId { get; }

        /// <summary>Gets the probed status.</summary>
        public HostStatus Status { get; }

        /// <summary>Gets the daemon version reported by the probe, or empty when unknown.</summary>
        public string Version { get; }
    }
}
