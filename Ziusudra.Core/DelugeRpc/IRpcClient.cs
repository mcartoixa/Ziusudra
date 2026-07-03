using System.Net;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Communicates with a Deluge server using the Deluge RPC protocol.</summary>
    /// <remarks>This is the seam the session/orchestration layer depends on, so it can be unit-tested
    /// against a fake without opening a socket. <see cref="RpcClient" /> is the production implementation.</remarks>
    public interface IRpcClient
    {

        /// <summary>Sends the specified <paramref name="request" /> to the server.</summary>
        /// <typeparam name="TResponse">The type of the response to expect.</typeparam>
        /// <param name="request">The request to send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The expected response from the server.</returns>
        ValueTask<TResponse> SendRequestAsync<TResponse>(RpcRequest<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse:
                RpcResponse;

        /// <summary>Starts the client.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        ValueTask StartAsync(CancellationToken cancellationToken = default);

        /// <summary>Stops the client.</summary>
        ValueTask StopAsync();

        /// <summary>Gets the current host.</summary>
        IPEndPoint Host { get; }

        /// <summary>Event triggered when an RPC event is received from the server.</summary>
        event EventHandler<RpcEventReceivedEventArgs>? RpcEventReceived;
    }
}
