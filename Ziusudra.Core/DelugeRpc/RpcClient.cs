using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
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
        public RpcClient(IPEndPoint host)
        {
            _Host = host;
        }

        /// <summary>Finalizer.</summary>
        ~RpcClient()
        {
            Dispose(false);
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
            if (_IsDisposed)
                throw new ObjectDisposedException(nameof(RpcClient));
            if (_Writer == null)
                throw new InvalidOperationException(SR.RpcClient_ClientNotStarted);

            TaskCompletionSource<IServerReply> requestTask = new();
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
                _ExpectedReplies.TryRemove(request.Id, out _);
                cancellationToken.ThrowIfCancellationRequested();
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
        /// <exception cref="ObjectDisposedException">Thrown if the client has already been stopped.</exception>
        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (_IsDisposed)
                throw new ObjectDisposedException(nameof(RpcClient));

            Socket socket = new(_Host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(_Host, cancellationToken)
                .ConfigureAwait(false);

            NetworkStream ns = new(socket, true);
            _Stream = new SslStream(ns, true);
            await _Stream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions() {
                CertificateRevocationCheckMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true, // Do not validate certificates
                TargetHost = _Host.Address.ToString()
            }, cancellationToken)
                .ConfigureAwait(false);

            _Writer = new RpcStreamWriter(_Stream, true) {
                Logger = Logger
            };

            _MessageLoopTask = Task.Run(() => ReceiveMessagesLoopAsync(_CancellationTokenSource.Token));
        }

        /// <summary>Stops the client.</summary>
        public ValueTask StopAsync()
        {
            return DisposeAsync();
        }

        private void OnRpcEventReceived(RpcEventReceivedEventArgs e)
        {
            RpcEventReceived?.Invoke(this, e);
        }

        private async ValueTask ReceiveMessagesLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_IsDisposed)
                    throw new ObjectDisposedException(nameof(RpcClient));
                if (_Stream == null)
                    throw new InvalidOperationException(SR.RpcClient_ClientNotStarted);
                if (_Reader == null)
                    _Reader = new RpcStreamReader(_Stream, true) {
                        Logger = Logger
                    };

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        IServerMessage message = await _Reader.ReadAsync(cancellationToken)
                            .ConfigureAwait(false);
                        if (message is IServerReply reply)
                        {
                            if (_ExpectedReplies.TryRemove(reply.Id, out TaskCompletionSource<IServerReply>? requestTask))
                            {
                                if (reply is RpcServerException error)
                                    requestTask.TrySetException(error);
                                else
                                    requestTask.TrySetResult(reply);
                            }  else
                                Logger.LogWarning("Deluge RPC response received without request: {0}", reply.Id);
                        } else if (message is RpcEvent @event)
                            OnRpcEventReceived(new RpcEventReceivedEventArgs(@event));
                        else
                            Logger.LogInformation("Deluge RPC message not recognized: {0}", message.ToDebugString());
                    } catch (OperationCanceledException)
                    {
                        throw;
                    } catch (Exception ex)
                    {
                        Logger.LogError(ex, "Deluge RPC error occured while receiving messages");
                    }
                }
            } catch (OperationCanceledException)
            { }
        }

        private void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_IsDisposed)
            {
                _CancellationTokenSource.Dispose();
                _ExpectedReplies.Clear();

                if (_Reader != null)
                    _Reader.Close();
                if (_Writer != null)
                    _Writer.Close();

                if (_Stream != null)
                    _Stream.Dispose();

                _IsDisposed = true;
            }

            _MessageLoopTask = null;
            _Reader = null;
            _Stream = null;
            _Writer = null;
        }

        private async ValueTask DisposeAsync()
        {
            if (!_IsDisposed) 
            {
                if (_MessageLoopTask != null)
                {
                    _CancellationTokenSource.Cancel();
                    await _MessageLoopTask
                        .ConfigureAwait(false);
                }

                if (_Stream != null)
                {
                    await _Stream.DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return DisposeAsync();
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
            }
        }

        /// <summary>Gets the current host.</summary>
        public IPEndPoint Host => _Host;

        /// <summary>Event triggered when an RPC event is received from the server.</summary>
        public event EventHandler<RpcEventReceivedEventArgs>? RpcEventReceived;

        private readonly CancellationTokenSource _CancellationTokenSource = new();
        private readonly ConcurrentDictionary<int, TaskCompletionSource<IServerReply>> _ExpectedReplies = new();
        private IPEndPoint _Host;
        private bool _IsDisposed;
        private ILogger _Logger = NullLogger.Instance;
        private Task? _MessageLoopTask;
        private RpcStreamReader? _Reader;
        private SslStream? _Stream;
        private RpcStreamWriter? _Writer;
    }
}
