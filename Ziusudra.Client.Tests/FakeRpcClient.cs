using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ziusudra.DelugeRpc;

namespace Ziusudra.Client.Tests
{

    /// <summary>A scripted <see cref="IRpcClient" /> that returns canned responses per RPC method, without any socket.</summary>
    /// <remarks>It builds real typed responses through the public <see cref="IClientRequest.CreateResponse(ICollection)" />
    /// seam, so the session sees exactly the response types it would get from the wire.</remarks>
    internal sealed class FakeRpcClient:
        IRpcClient
    {

        public FakeRpcClient(IPEndPoint host)
        {
            Host = host;
        }

        /// <summary>Scripts the value returned as the payload of the reply to the given <paramref name="method" />.</summary>
        public FakeRpcClient Respond(string method, object? returnValue)
        {
            _Responses[method] = () => returnValue;
            return this;
        }

        /// <summary>Scripts the given <paramref name="method" /> to fail with <paramref name="exception" />.</summary>
        public FakeRpcClient Fail(string method, Exception exception)
        {
            _Responses[method] = () => throw exception;
            return this;
        }

        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            IsStarted = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask StopAsync()
        {
            IsStopped = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask<TResponse> SendRequestAsync<TResponse>(RpcRequest<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse:
                RpcResponse
        {
            var clientRequest = (IClientRequest)request;
            SentMethods.Add(clientRequest.Method);
            if (!_Responses.TryGetValue(clientRequest.Method, out Func<object?>? factory))
                throw new InvalidOperationException($"No response scripted for '{clientRequest.Method}'.");

            object? value = factory();
            var values = new ArrayList { (int)RpcMessageType.RPC_RESPONSE, request.Id, value };
            IServerReply reply = clientRequest.CreateResponse(values);
            return ValueTask.FromResult((TResponse)reply);
        }

        public IPEndPoint Host { get; }

        public bool IsStarted { get; private set; }

        public bool IsStopped { get; private set; }

        public List<string> SentMethods { get; } = new();

        /// <summary>Raises <see cref="RpcEventReceived" /> with the given <paramref name="event" />, as the wire loop would.</summary>
        public void Raise(RpcEvent @event)
        {
            RpcEventReceived?.Invoke(this, new RpcEventReceivedEventArgs(@event));
        }

        public event EventHandler<RpcEventReceivedEventArgs>? RpcEventReceived;

        private readonly Dictionary<string, Func<object?>> _Responses = new(StringComparer.Ordinal);
    }
}
