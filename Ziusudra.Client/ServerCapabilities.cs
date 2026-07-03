namespace Ziusudra.Client
{

    /// <summary>An immutable snapshot of what a connected daemon supports, resolved once per connection.</summary>
    /// <remarks>The primary compatibility gate is method presence: a feature declares the RPC method it needs and
    /// checks <see cref="HasMethod" />. This is exact per connection and robust to forks and plugins.</remarks>
    public sealed class ServerCapabilities
    {

        /// <summary>Create a new instance of the <see cref="ServerCapabilities" /> type.</summary>
        /// <param name="daemonVersion">The daemon version reported by <c>daemon.info</c>.</param>
        /// <param name="libtorrentVersion">The libtorrent version, or an empty string when the daemon does not expose it.</param>
        /// <param name="methods">The RPC methods the daemon exposes, from <c>daemon.get_method_list</c>.</param>
        public ServerCapabilities(string daemonVersion, string libtorrentVersion, IEnumerable<string> methods)
        {
            ArgumentNullException.ThrowIfNull(daemonVersion);
            ArgumentNullException.ThrowIfNull(libtorrentVersion);
            ArgumentNullException.ThrowIfNull(methods);

            DaemonVersion = daemonVersion;
            LibtorrentVersion = libtorrentVersion;
            _Methods = new HashSet<string>(methods, StringComparer.Ordinal);
        }

        /// <summary>Determines whether the daemon exposes the specified RPC <paramref name="method" />.</summary>
        /// <param name="method">The fully qualified method name (for example <c>core.get_torrents_status</c>).</param>
        /// <returns><c>true</c> when the method is present; otherwise <c>false</c>.</returns>
        public bool HasMethod(string method)
        {
            ArgumentNullException.ThrowIfNull(method);
            return _Methods.Contains(method);
        }

        /// <summary>Gets the daemon version reported by <c>daemon.info</c>.</summary>
        public string DaemonVersion { get; }

        /// <summary>Gets the libtorrent version, or an empty string when the daemon does not expose it.</summary>
        public string LibtorrentVersion { get; }

        /// <summary>Gets the RPC methods the daemon exposes.</summary>
        public IReadOnlyCollection<string> Methods => _Methods;

        private readonly HashSet<string> _Methods;
    }
}
