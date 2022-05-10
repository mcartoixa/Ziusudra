namespace Ziusudra.DelugeRpc
{

    /// <summary>Provides data to the <see cref="RpcClient.RpcEventReceived" /> event.</summary>
    public class RpcEventReceivedEventArgs:
        EventArgs
    {

        /// <summary>Creates a new instance of the <see cref="RpcEventReceivedEventArgs" /> type.</summary>
        /// <param name="event"></param>
        internal RpcEventReceivedEventArgs(RpcEvent @event)
        {
            Event = @event;
        }

        /// <summary>The event sent by the server.</summary>
        public RpcEvent Event { get; private set; }
    }
}
