using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Client that can communicate with a Deluge server using the Deluge RPC protocol.</summary>
    public sealed class RpcClient:
        IDisposable,
        IAsyncDisposable
    {

        /// <summary>Create a new instance of the <see cref="RpcClient" /> class.</summary>
        /// <param name="host">The server to communicate with.</param>
        public RpcClient(IPEndPoint host):
            this(host, null)
        { }

        /// <summary>Create a new instance of the <see cref="RpcClient" /> class.</summary>
        /// <param name="host">The server to communicate with.</param>
        /// <param name="options">The options that configure the client, or <c>null</c> to use the defaults.</param>
        public RpcClient(IPEndPoint host, RpcClientOptions? options)
        {
            _Host = host;
            _Options = options ?? new RpcClientOptions();
        }

        /// <summary>Sends the specified <paramref name="request" /> to the server.</summary>
        /// <param name="request">The request to send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The expected response from the server.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the client has not been started.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the client has already been stopped.</exception>
        /// <exception cref="RpcServerException">Thrown when an error occured on the server while processing the request.</exception>
        public async ValueTask<IServerReply> SendRequestAsync(IClientRequest request, CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (_Writer == null)
                throw new InvalidOperationException(SR.RpcClient_ClientNotStarted);

            TaskCompletionSource<IServerReply> requestTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
            if (!_ExpectedReplies.TryAdd(request.Id, requestTask))
                throw new InvalidOperationException(SR.RpcClient_CouldNotWaitForAResponse);
            try
            {
                await _Writer.WriteAsync(request, cancellationToken)
                    .ConfigureAwait(false);
            } catch
            {
                _ExpectedReplies.TryRemove(request.Id, out _);
                throw;
            }

            using CancellationTokenRegistration ctr = cancellationToken.Register(() => {
                if (_ExpectedReplies.TryRemove(request.Id, out TaskCompletionSource<IServerReply>? task))
                    task.TrySetCanceled(cancellationToken);
            });
            return await requestTask.Task
                .ConfigureAwait(false);
        }

        /// <summary>Sends the specified <paramref name="request" /> to the server.</summary>
        /// <typeparam name="TResponse">The type of the response to expect.</typeparam>
        /// <param name="request">The request to send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The expected response from the server.</returns>
        public async ValueTask<TResponse> SendRequestAsync<TResponse>(RpcRequest<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse:
                RpcResponse
        {
            IServerReply reply = await SendRequestAsync((IClientRequest)request, cancellationToken)
                .ConfigureAwait(false);
            return request.CreateResponse(reply);
        }

        /// <summary>Starts the client.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">Thrown if the client has already been started.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the client has already been stopped.</exception>
        [SuppressMessage("Security", "CA5359:Do not disable certificate validation", Justification = "Deluge daemons use a self-signed certificate and the reference client does not validate it; validation is opt-in through RpcClientOptions.CertificateValidationCallback.")]
        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (_MessageLoopTask != null)
                throw new InvalidOperationException(SR.RpcClient_ClientAlreadyStarted);

            Socket socket = new(_Host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(_Host, cancellationToken)
                .ConfigureAwait(false);

            NetworkStream ns = new(socket, true);
            _Stream = new SslStream(ns, false);
            await _Stream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions() {
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                RemoteCertificateValidationCallback = _Options.CertificateValidationCallback ?? AcceptAnyCertificate,
                TargetHost = _Host.Address.ToString()
            }, cancellationToken)
                .ConfigureAwait(false);

            _Writer = new RpcStreamWriter(_Stream, true) {
                Logger = Logger
            };
            _Reader = new RpcStreamReader(_Stream, true) {
                Logger = Logger,
                MaxFrameSize = _Options.MaxFrameSize
            };

            RpcMessageLoop loop = new(
                _Reader,
                _ExpectedReplies,
                @event => OnRpcEventReceived(new RpcEventReceivedEventArgs(@event)),
                Logger);
            _MessageLoopTask = Task.Run(() => loop.RunAsync(_CancellationTokenSource.Token), _CancellationTokenSource.Token);
        }

        /// <summary>Stops the client.</summary>
        public ValueTask StopAsync()
        {
            return DisposeAsyncCore();
        }

        private static bool AcceptAnyCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void OnRpcEventReceived(RpcEventReceivedEventArgs e)
        {
            RpcEventReceived?.Invoke(this, e);
        }

        private void FaultPendingReplies(Exception exception)
        {
            foreach (int id in _ExpectedReplies.Keys)
                if (_ExpectedReplies.TryRemove(id, out TaskCompletionSource<IServerReply>? requestTask))
                    requestTask.TrySetException(exception);
        }

        private void Cleanup()
        {
            FaultPendingReplies(new ObjectDisposedException(nameof(RpcClient)));

            _Reader?.Close();
            _Writer?.Close();

            _MessageLoopTask = null;
            _Reader = null;
            _Stream = null;
            _Writer = null;
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (Interlocked.Exchange(ref _Disposed, 1) != 0)
                return;

            _CancellationTokenSource.Cancel();
            if (_MessageLoopTask != null)
            {
                try
                {
                    await _MessageLoopTask
                        .ConfigureAwait(false);
                } catch
                { }
            }
            if (_Stream != null)
                await _Stream.DisposeAsync()
                    .ConfigureAwait(false);

            Cleanup();
            // Safe to dispose now that the loop has been awaited and no longer touches the token.
            _CancellationTokenSource.Dispose();
        }

        void IDisposable.Dispose()
        {
            if (Interlocked.Exchange(ref _Disposed, 1) != 0)
                return;
            GC.SuppressFinalize(this);

            // Cancel and dispose the stream so the message loop's pending read unblocks and the loop exits.
            // The synchronous path cannot await the loop, unlike DisposeAsync.
            _CancellationTokenSource.Cancel();
            _Stream?.Dispose();

            Cleanup();
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return DisposeAsyncCore();
        }

        /// <summary>Get or set the current logger.</summary>
        public ILogger Logger
        {
            get
            {
                return _Logger;
            }
            set
            {
                _Logger = value;
                if (_Reader != null)
                    _Reader.Logger = value;
                if (_Writer != null)
                    _Writer.Logger = value;
            }
        }

        /// <summary>Gets the current host.</summary>
        public IPEndPoint Host => _Host;

        /// <summary>Event triggered when an RPC event is received from the server.</summary>
        public event EventHandler<RpcEventReceivedEventArgs>? RpcEventReceived;

        private bool IsDisposed => Volatile.Read(ref _Disposed) != 0;

        private readonly CancellationTokenSource _CancellationTokenSource = new();
        private int _Disposed;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<IServerReply>> _ExpectedReplies = new();
        private readonly IPEndPoint _Host;
        private ILogger _Logger = NullLogger.Instance;
        private Task? _MessageLoopTask;
        private readonly RpcClientOptions _Options;
        private RpcStreamReader? _Reader;
        private SslStream? _Stream;
        private RpcStreamWriter? _Writer;
    }
}
