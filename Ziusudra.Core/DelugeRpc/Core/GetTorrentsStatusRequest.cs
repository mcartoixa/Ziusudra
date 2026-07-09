using System.Collections;
using System.Collections.Specialized;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Request for all torrents.</summary>
    public class GetTorrentsStatusRequest:
        RpcRequest<GetTorrentsStatusRequest.Response>
    {

        /// <summary>Response to a <see cref="GetTorrentsStatusRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets the torrents returned by the server.</summary>
            public IEnumerable<Torrent> Torrents => GetTorrentsFromValues(Values[2] as IDictionary);

            private static IEnumerable<Torrent> GetTorrentsFromValues(IDictionary? values)
            {
                if (values is null)
                    return Array.Empty<Torrent>();
                return values.Cast<DictionaryEntry>()
                        .Select(de => new Torrent(de.Key.ToString() ?? string.Empty, de.Value as IDictionary ?? new ListDictionary()));
            }
        }

        /// <summary>Create a new instance of the <see cref="GetTorrentsStatusRequest" /> type.</summary>
        public GetTorrentsStatusRequest()
        {
            _Keys = new ArrayList();
            _Filter = new ListDictionary();
        }

        /// <summary>Create a new instance of the <see cref="GetTorrentsStatusRequest" /> type.</summary>
        /// <param name="keys">The keys to get the status for.</param>
        public GetTorrentsStatusRequest(IEnumerable<string> keys)
        {
            _Keys = keys.ToArray();
            _Filter = new ListDictionary();
        }

        /// <summary>Create a new instance of the <see cref="GetTorrentsStatusRequest" /> type restricted to a filter.</summary>
        /// <param name="filter">The filter to narrow the torrents by, keyed by filter category (for example <c>state</c>). An empty filter returns every torrent.</param>
        /// <param name="keys">The keys to get the status for.</param>
        public GetTorrentsStatusRequest(IReadOnlyDictionary<string, string> filter, IEnumerable<string> keys)
        {
            ArgumentNullException.ThrowIfNull(filter);
            _Keys = keys.ToArray();
            _Filter = BuildFilter(filter);
        }

        /// <summary>Create the typed response to the current request from the specified server <paramref name="reply" />.</summary>
        /// <param name="reply">The reply to create the response from.</param>
        /// <returns>The response to the current request.</returns>
        protected internal override Response CreateResponse(IServerReply reply)
        {
            return new Response(reply);
        }

        /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The filter dictionary followed by the requested status keys.</returns>
        protected override ICollection GetArgs()
        {
            return new object[] { _Filter, _Keys };
        }

        private static IDictionary BuildFilter(IReadOnlyDictionary<string, string> filter)
        {
            var result = new ListDictionary();
            foreach (KeyValuePair<string, string> pair in filter)
                result[pair.Key] = pair.Value;
            return result;
        }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => "core.get_torrents_status";

        private readonly ICollection _Keys;
        private readonly IDictionary _Filter;
    }
}
