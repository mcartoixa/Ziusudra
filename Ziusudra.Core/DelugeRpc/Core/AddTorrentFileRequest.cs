using System.Collections;
using System.Collections.Specialized;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Adds a torrent file to the session.</summary>
    public class AddTorrentFileRequest:
        RpcRequest<AddTorrentFileRequest.Response>
    {

        /// <summary>Response to an <see cref="AddTorrentFileRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets the identifier of the added torrent, or <c>null</c>.</summary>
            public string? TorrentId => Values[2]?.ToString();
        }

        /// <summary>Create a new instance of the <see cref="AddTorrentFileRequest" /> type.</summary>
        /// <param name="fileName">The name of the torrent file.</param>
        /// <param name="fileContent">The raw contents of the torrent file; the daemon expects them base64-encoded, which this request does.</param>
        public AddTorrentFileRequest(string fileName, byte[] fileContent)
        {
            ArgumentNullException.ThrowIfNull(fileContent);
            _FileName = fileName;
            _FileDump = Convert.ToBase64String(fileContent);
        }

        /// <summary>Create the typed response to the current request from the specified server <paramref name="reply" />.</summary>
        /// <param name="reply">The reply to create the response from.</param>
        /// <returns>The response to the current request.</returns>
        protected internal override Response CreateResponse(IServerReply reply)
        {
            return new Response(reply);
        }

        /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The arguments.</returns>
        protected override ICollection GetArgs()
        {
            return new object[] { _FileName, _FileDump, new ListDictionary() };
        }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => MethodName;

        /// <summary>The name of the remote method called by this request.</summary>
        public const string MethodName = "core.add_torrent_file";

        private readonly string _FileName;
        private readonly string _FileDump;
    }
}
