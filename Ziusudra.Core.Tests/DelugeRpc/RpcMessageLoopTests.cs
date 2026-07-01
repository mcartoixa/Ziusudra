using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Ziusudra.DelugeRpc.Tests
{

    public class RpcMessageLoopTests
    {

        [Fact]
        public async Task RunAsync_ShouldDispatchAReplyToTheMatchingRequest()
        {
            var pending = new ConcurrentDictionary<int, TaskCompletionSource<IServerReply>>();
            TaskCompletionSource<IServerReply> request = NewRequest();
            pending[1] = request;

            var reader = new ScriptedServerMessageReader(
                () => Response(1),
                () => throw new IOException());
            var loop = new RpcMessageLoop(reader, pending, _ => { }, NullLogger.Instance);

            await loop.RunAsync(CancellationToken.None);

            Assert.Equal(1, Assert.IsType<RpcResponse>(await request.Task).Id);
        }

        [Fact]
        public async Task RunAsync_ShouldFaultPendingRequestsWhenTheConnectionIsLost()
        {
            var pending = new ConcurrentDictionary<int, TaskCompletionSource<IServerReply>>();
            TaskCompletionSource<IServerReply> request = NewRequest();
            pending[7] = request;

            var reader = new ScriptedServerMessageReader(() => throw new IOException());
            var loop = new RpcMessageLoop(reader, pending, _ => { }, NullLogger.Instance);

            await loop.RunAsync(CancellationToken.None);

            await Assert.ThrowsAsync<RpcException>(async () => await request.Task);
            Assert.Empty(pending);
        }

        [Fact]
        public async Task RunAsync_ShouldSkipAnUndecodableMessageAndKeepGoing()
        {
            var pending = new ConcurrentDictionary<int, TaskCompletionSource<IServerReply>>();
            TaskCompletionSource<IServerReply> request = NewRequest();
            pending[1] = request;

            // A decode error on a fully received frame is non-fatal: the loop skips it and dispatches the next reply.
            var reader = new ScriptedServerMessageReader(
                () => throw new Rencode.RencodeException("malformed"),
                () => Response(1),
                () => throw new IOException());
            var loop = new RpcMessageLoop(reader, pending, _ => { }, NullLogger.Instance);

            await loop.RunAsync(CancellationToken.None);

            Assert.Equal(1, Assert.IsType<RpcResponse>(await request.Task).Id);
        }

        [Fact]
        public async Task RunAsync_ShouldRaiseReceivedEvents()
        {
            var pending = new ConcurrentDictionary<int, TaskCompletionSource<IServerReply>>();
            var received = new List<RpcEvent>();

            var reader = new ScriptedServerMessageReader(
                () => Event("TorrentAddedEvent"),
                () => throw new IOException());
            var loop = new RpcMessageLoop(reader, pending, received.Add, NullLogger.Instance);

            await loop.RunAsync(CancellationToken.None);

            Assert.Single(received);
        }

        [Fact]
        public async Task RunAsync_ShouldStopWithoutFaultingPendingRequestsWhenCancelled()
        {
            var pending = new ConcurrentDictionary<int, TaskCompletionSource<IServerReply>>();
            TaskCompletionSource<IServerReply> dispatched = NewRequest();
            TaskCompletionSource<IServerReply> stillPending = NewRequest();
            pending[1] = dispatched;
            pending[2] = stillPending;

            using var cts = new CancellationTokenSource();
            var reader = new ScriptedServerMessageReader(() => Response(1));
            var loop = new RpcMessageLoop(reader, pending, _ => { }, NullLogger.Instance);

            Task run = loop.RunAsync(cts.Token);
            await dispatched.Task.WaitAsync(TimeSpan.FromSeconds(5));
            cts.Cancel();
            await run.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.False(stillPending.Task.IsCompleted);
        }

        private static TaskCompletionSource<IServerReply> NewRequest()
        {
            return new TaskCompletionSource<IServerReply>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        private static IServerMessage Response(int id)
        {
            return new RpcResponse(new ArrayList { (int)RpcMessageType.RPC_RESPONSE, id, "ok" });
        }

        private static IServerMessage Event(string name)
        {
            return RpcEvent.CreateFromValues(new ArrayList { (int)RpcMessageType.RPC_EVENT, name, new ArrayList() });
        }
    }

    /// <summary>A reader that plays a fixed script of steps, then blocks until cancelled.</summary>
    /// <remarks>Each step either returns the next message or throws to simulate a read failure.</remarks>
    internal sealed class ScriptedServerMessageReader:
        IServerMessageReader
    {

        public ScriptedServerMessageReader(params Func<IServerMessage>[] steps)
        {
            _Steps = new Queue<Func<IServerMessage>>(steps);
        }

        public ValueTask<IServerMessage> ReadAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return ValueTask.FromCanceled<IServerMessage>(cancellationToken);

            if (_Steps.Count == 0)
            {
                var blocked = new TaskCompletionSource<IServerMessage>();
                cancellationToken.Register(() => blocked.TrySetCanceled(cancellationToken));
                return new ValueTask<IServerMessage>(blocked.Task);
            }

            Func<IServerMessage> step = _Steps.Dequeue();
            try
            {
                return new ValueTask<IServerMessage>(step());
            } catch (Exception ex)
            {
                return ValueTask.FromException<IServerMessage>(ex);
            }
        }

        private readonly Queue<Func<IServerMessage>> _Steps;
    }
}
