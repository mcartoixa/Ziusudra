namespace Ziusudra.Client
{

    /// <summary>The reachability of a saved host, as resolved by probing it.</summary>
    public enum HostStatus
    {
        /// <summary>The host has not been probed yet.</summary>
        Unknown,
        /// <summary>The host could not be reached.</summary>
        Offline,
        /// <summary>The daemon answered a probe but no session is connected to it.</summary>
        Online,
        /// <summary>The current session is connected to this host.</summary>
        Connected
    }
}
