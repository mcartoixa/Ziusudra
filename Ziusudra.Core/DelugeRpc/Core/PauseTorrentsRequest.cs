using System.Collections;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Pauses a list of torrents.</summary>
    public class PauseTorrentsRequest:
        RpcRequest<PauseTorrentsRequest.Response>
    {

        /// <summary>Response to a <see cref="PauseTorrentsRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }
        }

        /// <summary>Create a new instance of the <see cref="PauseTorrentsRequest" /> type.</summary>
        /// <param name="torrentIds">The identifiers of the torrents to pause.</param>
        public PauseTorrentsRequest(IEnumerable<string> torrentIds)
        {
            _TorrentIds = torrentIds.ToArray();
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
            return new object[] { _TorrentIds };
        }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => MethodName;

        /// <summary>The name of the remote method called by this request.</summary>
        public const string MethodName = "core.pause_torrents";

        private readonly string[] _TorrentIds;
    }
}
