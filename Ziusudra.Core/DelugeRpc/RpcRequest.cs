using System.Collections;
using System.Collections.Specialized;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Base class for client requests.</summary>
    /// <typeparam name="TResponse">The type of the expected response associated to the request.</typeparam>
    public abstract class RpcRequest<TResponse>:
        IClientRequest
        where TResponse:
            RpcResponse
    {

        /// <summary>Create a new instance of the <see cref="RpcRequest{TResponse}" /> type.</summary>
        public RpcRequest()
        { }

        /// <summary>Create the typed response to the current request from the specified server <paramref name="reply" />.</summary>
        /// <param name="reply">The reply to create the response from.</param>
        /// <returns>The response to the current request.</returns>
        internal protected abstract TResponse CreateResponse(IServerReply reply);

        /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The arguments.</returns>
        protected abstract ICollection GetArgs();

        /// <summary>Gets the keyword arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The keyword arguments.</returns>
        protected virtual IDictionary<string, object> GetKeywordArgs()
        {
            return new Dictionary<string, object>();
        }

        IServerReply IClientRequest.CreateResponse(ICollection values)
        {
            return CreateResponse(new RpcResponse(values));
        }

        IDictionary IClientRequest.GetKeywordArgs()
        {
            var args = GetKeywordArgs();
            if (args is not IDictionary d)
            {
                d = new HybridDictionary();
                foreach (var kvp in args)
                    d.Add(kvp.Key, kvp.Value);
            }
            return d;
        }

        ICollection IMessage.ToValueCollection()
        {
            var args = new ArrayList();
            args.AddRange(new object[] { Id, Method, GetArgs() });
            var kwargs = ((IClientRequest)this).GetKeywordArgs();
            args.Add(kwargs ?? new Dictionary<string, object>());

            var ret = new ArrayList();
            ret.Add(args);
            return ret;
        }

        ICollection IClientRequest.GetArgs() => GetArgs();

        /// <summary>Gets the identifier of the request.</summary>
        /// <remarks>Every new instance will have a different identifier,
        /// used to associate sent requests to received responses.</remarks>
        public int Id { get; } = _Random.Next();

        /// <summary>Gets the name of the remote method to call.</summary>
        protected abstract string Method { get; }

        string IClientRequest.Method => Method;

        private static readonly Random _Random = new();
    }
}
