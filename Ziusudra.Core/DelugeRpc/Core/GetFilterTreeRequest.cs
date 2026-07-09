using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Request for current filters.</summary>
    public class GetFilterTreeRequest:
        RpcRequest<GetFilterTreeRequest.Response>
    {

        /// <summary>Response to a <see cref="GetFilterTreeRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets the filters returned by the server, keyed by category.</summary>
            public IDictionary<string, ICollection<Filter>> Filters => GetFiltersFromValues(Values[2] as IDictionary);

            private static IDictionary<string, ICollection<Filter>> GetFiltersFromValues(IDictionary? values)
            {
                if (values is null)
                    return new Dictionary<string, ICollection<Filter>>();
                return values.Cast<DictionaryEntry>()
                    .Select(de => new KeyValuePair<string, ICollection<Filter>>(
                        de.Key.ToString() ?? string.Empty,
                        (de.Value as ICollection ?? new ArrayList())
                            .Cast<ICollection>()
                            .Select(f => new Filter(de.Key.ToString() ?? string.Empty, f))
                            .ToArray()
                    ))
                    .ToDictionary();
            }
        }

        /// <summary>Create a new instance of the <see cref="GetFilterTreeRequest" /> type.</summary>
        public GetFilterTreeRequest()
        { }

        /// <summary>Create a new instance of the <see cref="GetFilterTreeRequest" /> type.</summary>
        /// <param name="showZeroHits">Whether to show filters with no corresponding torrents.</param>
        /// <param name="hiddenCategories">A list of categories to hide.</param>
        public GetFilterTreeRequest(bool showZeroHits, IEnumerable<string>? hiddenCategories)
        {
            _HiddenCategories = hiddenCategories?.ToArray();
            _ShowZeroHits = showZeroHits;
        }

        /// <summary>Create the typed response to the current request from the specified server <paramref name="reply" />.</summary>
        /// <param name="reply">The reply to create the response from.</param>
        /// <returns>The response to the current request.</returns>
        protected internal override Response CreateResponse(IServerReply reply)
        {
            return new Response(reply);
        }

        /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>An empty collection.</returns>
        protected override ICollection GetArgs()
        {
            return new object?[] { _ShowZeroHits, _HiddenCategories };
        }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => MethodName;

        /// <summary>The name of the remote method called by this request.</summary>
        public const string MethodName = "core.get_filter_tree";

        private readonly ICollection? _HiddenCategories;
        private readonly bool? _ShowZeroHits;
    }
}
