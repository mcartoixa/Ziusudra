namespace Ziusudra.Client
{

    /// <summary>The lifecycle state of a <see cref="DelugeSession" />.</summary>
    public enum SessionState
    {
        /// <summary>Not connected to a daemon.</summary>
        Disconnected,
        /// <summary>Establishing the transport (TCP and TLS) to the daemon.</summary>
        Connecting,
        /// <summary>Authenticating with the daemon.</summary>
        Authenticating,
        /// <summary>Connected and authenticated; ready to serve requests.</summary>
        Connected,
        /// <summary>A transport or protocol fault has broken the session.</summary>
        Faulted
    }
}
