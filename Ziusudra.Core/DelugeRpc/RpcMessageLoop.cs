using System.Collections.Concurrent;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Reads messages from a server and dispatches them to the pending requests and event handlers.</summary>
    /// <remarks>A transport failure (the connection being lost) stops the loop and faults every pending request. A failure to
    /// decode a single, fully received frame is logged and skipped, leaving the connection usable, mirroring the behaviour of
    /// the Deluge daemon itself.</remarks>
    internal sealed class RpcMessageLoop
    {

        /// <summary>Create a new instance of the <see cref="RpcMessageLoop" /> type.</summary>
        /// <param name="reader">The reader to read messages from.</param>
        /// <param name="pendingReplies">The requests awaiting a reply, keyed by request identifier.</param>
        /// <param name="eventReceived">Invoked when an event is received from the server.</param>
        /// <param name="logger">The logger.</param>
        public RpcMessageLoop(
            IServerMessageReader reader,
            ConcurrentDictionary<int, TaskCompletionSource<IServerReply>> pendingReplies,
            Action<RpcEvent> eventReceived,
            ILogger logger)
        {
            _Reader = reader;
            _PendingReplies = pendingReplies;
            _EventReceived = eventReceived;
            _Logger = logger;
        }

        /// <summary>Reads and dispatches messages until the connection is lost or <paramref name="cancellationToken" /> is cancelled.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                IServerMessage message;
                try
                {
                    message = await _Reader.ReadAsync(cancellationToken)
                        .ConfigureAwait(false);
                } catch (OperationCanceledException)
                {
                    return;
                } catch (Exception ex) when (IsConnectionLost(ex))
                {
                    _Logger.LogError(ex, "Deluge RPC connection lost");
                    FaultPendingReplies(new RpcException(SR.RpcClient_ConnectionLost, ex));
                    return;
                } catch (Exception ex)
                {
                    _Logger.LogWarning(ex, "Deluge RPC message could not be decoded and was skipped");
                    continue;
                }

                Dispatch(message);
            }
        }

        private void Dispatch(IServerMessage message)
        {
            if (message is IServerReply reply)
            {
                if (_PendingReplies.TryRemove(reply.Id, out TaskCompletionSource<IServerReply>? requestTask))
                {
                    if (reply is RpcServerException error)
                        requestTask.TrySetException(error);
                    else
                        requestTask.TrySetResult(reply);
                } else
                    _Logger.LogWarning("Deluge RPC response received without a matching request: {Id}", reply.Id);
            } else if (message is RpcEvent @event)
                _EventReceived(@event);
            else if (_Logger.IsEnabled(LogLevel.Information))
                _Logger.LogInformation("Deluge RPC message not recognized: {Message}", message.ToDebugString());
        }

        private void FaultPendingReplies(Exception exception)
        {
            foreach (int id in _PendingReplies.Keys)
                if (_PendingReplies.TryRemove(id, out TaskCompletionSource<IServerReply>? requestTask))
                    requestTask.TrySetException(exception);
        }

        private static bool IsConnectionLost(Exception exception)
        {
            return exception is EndOfStreamException
                or IOException
                or SocketException
                or ObjectDisposedException;
        }

        private readonly Action<RpcEvent> _EventReceived;
        private readonly ILogger _Logger;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<IServerReply>> _PendingReplies;
        private readonly IServerMessageReader _Reader;
    }
}
