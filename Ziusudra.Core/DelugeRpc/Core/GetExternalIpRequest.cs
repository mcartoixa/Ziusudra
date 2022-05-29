using System.Collections;

namespace Ziusudra.DelugeRpc.Core
{

    /// <summary>GetExternalIpRequest request.</summary>
    public class GetExternalIpRequest:
        RpcRequest<GetExternalIpRequest.Response>
    {

        /// <summary>Response to a <see cref="GetExternalIpRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets the version returned by the server.</summary>
            public string ExternalIp => Values[2] as string ?? string.Empty;
        }

        /// <summary>Create a new instance of the <see cref="GetExternalIpRequest" /> type.</summary>
        public GetExternalIpRequest()
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
        protected override string Method => "core.get_external_ip";
    }
}
