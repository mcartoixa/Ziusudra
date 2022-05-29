using System.Collections;
using System.Collections.Specialized;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>Returns a list of torrent identifiers in the session.</summary>
    public class GetConfig:
        RpcRequest<GetConfig.Response>
    {

        /// <summary>Response to a <see cref="GetConfig" />.</summary>
        public class Response:
            RpcResponse
    {

        /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
        /// <param name="reply">The reply to create the response from.</param>
        internal Response(IServerReply reply):
            base(reply.ToValueCollection())
        { }

        /// <summary>Gets the torrent identifiers in the session.</summary>
        public IDictionary Config => Values[2] as IDictionary ?? new ListDictionary();
    }

        /// <summary>Create a new instance of the <see cref="GetConfig" /> type.</summary>
        public GetConfig()
    { }

    /// <summary>Create the typed response to the current request from the specified server <paramref name="reply" />.</summary>
    /// <param name="reply">The reply to create the response from.</param>
    /// <returns>The response to the current request.</returns>
    internal protected override Response CreateResponse(IServerReply reply)
    {
        return new Response(reply);
    }

    /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
    /// <returns>An empty collection.</returns>
    protected override ICollection GetArgs()
    {
        return Array.Empty<string>();
    }

    /// <summary>Gets the name of the remote method to call.</summary>
    protected override string Method => "core.get_config";
}
}
