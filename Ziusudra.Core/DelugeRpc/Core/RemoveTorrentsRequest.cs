using System.Collections;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Removes several torrents from the session in a single request.</summary>
    /// <remarks>The daemon removes what it can and reports the failures, saving the session state once for the whole
    /// batch — unlike calling <see cref="RemoveTorrentRequest" /> per torrent.</remarks>
    public class RemoveTorrentsRequest:
        RpcRequest<RemoveTorrentsRequest.Response>
    {

        /// <summary>Response to a <see cref="RemoveTorrentsRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets the torrents that could not be removed; empty when every torrent was removed.</summary>
            public IEnumerable<RemoveTorrentError> Errors => GetErrorsFromValues(Values[2] as IEnumerable);

            private static IEnumerable<RemoveTorrentError> GetErrorsFromValues(IEnumerable? values)
            {
                if (values is null)
                    return Array.Empty<RemoveTorrentError>();
                return values.OfType<IList>()
                        .Select(pair => new RemoveTorrentError(
                            (pair.Count > 0 ? pair[0]?.ToString() : null) ?? string.Empty,
                            (pair.Count > 1 ? pair[1]?.ToString() : null) ?? string.Empty));
            }
        }

        /// <summary>Create a new instance of the <see cref="RemoveTorrentsRequest" /> type.</summary>
        /// <param name="torrentIds">The identifiers of the torrents to remove.</param>
        /// <param name="removeData">Whether to also remove the downloaded data from disk.</param>
        public RemoveTorrentsRequest(IEnumerable<string> torrentIds, bool removeData)
        {
            _TorrentIds = torrentIds.ToArray();
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
            return new object[] { _TorrentIds, _RemoveData };
        }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => MethodName;

        /// <summary>The name of the remote method called by this request.</summary>
        public const string MethodName = "core.remove_torrents";

        private readonly string[] _TorrentIds;
        private readonly bool _RemoveData;
    }
}
