using System.Collections;
using System.Globalization;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Removes a single torrent from the session.</summary>
    public class RemoveTorrentRequest:
        RpcRequest<RemoveTorrentRequest.Response>
    {

        /// <summary>Response to a <see cref="RemoveTorrentRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets a value indicating whether the torrent was removed.</summary>
            public bool Success => Convert.ToBoolean(Values[2], CultureInfo.InvariantCulture);
        }

        /// <summary>Create a new instance of the <see cref="RemoveTorrentRequest" /> type.</summary>
        /// <param name="torrentId">The identifier of the torrent to remove.</param>
        /// <param name="removeData">Whether to also remove the downloaded data from disk.</param>
        public RemoveTorrentRequest(string torrentId, bool removeData)
        {
            _TorrentId = torrentId;
            _RemoveData = removeData;
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
            return new object[] { _TorrentId, _RemoveData };
        }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => MethodName;

        /// <summary>The name of the remote method called by this request.</summary>
        public const string MethodName = "core.remove_torrent";

        private readonly string _TorrentId;
        private readonly bool _RemoveData;
    }
}
