using System.Net;

namespace Ziusudra.Client
{

    /// <summary>Resolves a host name and port to an <see cref="IPEndPoint" />.</summary>
    /// <remarks>Core takes an already-resolved <see cref="IPEndPoint" />, so resolving the stored host name is the
    /// client layer's job. This seam keeps the connection flow testable without hitting DNS.</remarks>
    public interface IHostResolver
    {

        /// <summary>Resolves the specified <paramref name="host" /> and <paramref name="port" /> to an endpoint.</summary>
        /// <param name="host">The host name or address.</param>
        /// <param name="port">The port.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resolved endpoint.</returns>
        Task<IPEndPoint> ResolveAsync(string host, int port, CancellationToken cancellationToken = default);
    }
}
