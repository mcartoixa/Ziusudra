using System.Net.Security;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Options that configure an <see cref="RpcClient" />.</summary>
    public sealed class RpcClientOptions
    {

        /// <summary>Gets or sets the callback used to validate the server certificate.</summary>
        /// <remarks>When <c>null</c>, any certificate is accepted. Deluge daemons use a self-signed certificate and the
        /// reference client does not validate it either, so accepting any certificate is the default. Supply a callback to
        /// validate or pin the expected certificate.</remarks>
        public RemoteCertificateValidationCallback? CertificateValidationCallback { get; set; }

        /// <summary>Gets or sets the maximum accepted size, in bytes, of a single compressed message frame.</summary>
        public int MaxFrameSize { get; set; } = RpcStreamReader.DefaultMaxFrameSize;
    }
}
