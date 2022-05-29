using System.Collections;

namespace Ziusudra.DelugeRpc.Daemon
{

    /// <summary>Login request.</summary>
    public class LoginRequest:
        RpcRequest<LoginRequest.Response>
    {

        /// <summary>Response to a <see cref="LoginRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply) :
                base(reply.ToValueCollection())
            { }

            /// <summary>Gets the result of the login operation.</summary>
            public int AuthenticationLevel => Convert.ToInt32(Values[2]);
        }

        /// <summary>Create a new instance of the <see cref="LoginRequest" /> type.</summary>
        /// <param name="username">The login username.</param>
        /// <param name="password">The login password</param>
        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>Create the typed response to the current request from the specified server <paramref name="reply" />.</summary>
        /// <param name="reply">The reply to create the response from.</param>
        /// <returns>The response to the current request.</returns>
        internal protected override Response CreateResponse(IServerReply reply)
        {
            return new Response(reply);
        }

        /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The arguments.</returns>
        protected override ICollection GetArgs()
        {
            return new string[] { Username, Password };
        }

        /// <summary>Gets the keyword arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The keyword arguments.</returns>
        protected override IDictionary<string, object> GetKeywordArgs()
        {
            return new Dictionary<string, object>() {
                { "client_version", ClientVersion } // Required for some reaseon
            };
        }

        /// <summary>Gets the password of the current request.</summary>
        public string Password { get; }
        /// <summary>Gets the username of the current request.</summary>
        public string Username { get; }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => "daemon.login";

        /// <summary>The client version.</summary>
        public static string ClientVersion => "Ziusudra";
    }
}
