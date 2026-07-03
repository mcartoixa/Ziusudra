using System.Globalization;
using System.Net;

namespace Ziusudra.Client
{

    /// <summary>Resolves host names using DNS.</summary>
    public sealed class DnsHostResolver:
        IHostResolver
    {

        /// <summary>Resolves the specified <paramref name="host" /> and <paramref name="port" /> to an endpoint.</summary>
        /// <param name="host">The host name or address.</param>
        /// <param name="port">The port.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resolved endpoint, using the first resolved address.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the host resolves to no addresses.</exception>
        public async Task<IPEndPoint> ResolveAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(host);

            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host, cancellationToken)
                .ConfigureAwait(false);
            if (addresses.Length == 0)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.DnsHostResolver_CouldNotResolveHost, host));

            return new IPEndPoint(addresses[0], port);
        }
    }
}
