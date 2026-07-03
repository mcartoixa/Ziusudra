namespace Ziusudra.Client
{

    /// <summary>A saved Deluge daemon connection target.</summary>
    /// <remarks>Credentials are per entry so one Windows user can connect as different Deluge accounts. The password is
    /// deliberately not stored here yet — it is supplied at connect time until secure credential persistence is added.</remarks>
    public sealed class HostEntry
    {

        /// <summary>The default Deluge daemon port.</summary>
        public const int DefaultPort = 58846;

        /// <summary>Gets or sets the stable identifier of the entry.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Gets or sets the display name of the entry.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the host name or address of the daemon.</summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>Gets or sets the daemon port.</summary>
        public int Port { get; set; } = DefaultPort;

        /// <summary>Gets or sets the account user name.</summary>
        public string Username { get; set; } = string.Empty;
    }
}
