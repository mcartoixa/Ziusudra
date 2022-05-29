using System.Collections;
using System.Globalization;

namespace Ziusudra.DelugeRpc.Daemon
{

    /// <summary>Info request.</summary>
    public class SetEventInterestRequest:
        RpcRequest<SetEventInterestRequest.Response>
    {

        /// <summary>Response to an <see cref="SetEventInterestRequest" />.</summary>
        public class Response:
            RpcResponse
        {

            /// <summary>Create an instance of the <see cref="Response" /> type.</summary>
            /// <param name="reply">The reply to create the response from.</param>
            internal Response(IServerReply reply):
                base(reply.ToValueCollection())
            { }
        }

        /// <summary>Create a new instance of the <see cref="SetEventInterestRequest" /> type.</summary>
        public SetEventInterestRequest():
            this(RpcEvent.TypedEventsList)
        { }

        /// <summary>Create a new instance of the <see cref="SetEventInterestRequest" /> type.</summary>
        public SetEventInterestRequest(IEnumerable<string> events)
        {
            Events = events.Distinct();
        }

        /// <summary>Create a new instance of the <see cref="SetEventInterestRequest" /> type.</summary>
        /// <param name="events">A list of typed events to susbcribe to.</param>
        /// <exception cref="AggregateException">Thrown when one or more of the specified <paramref name="events" /> are not proper typed events.</exception>
        public SetEventInterestRequest(IEnumerable<Type> events):
            this(events.Select(t => t.Name))
        {
            var exceptions = events.Where(t => !RpcEvent.IsTypedEvent(t))
                .Select(t => new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, SR.RpcEvent_TypeIsNotTypedEvent, t),
                    nameof(events)
                ));
            if (exceptions.Any())
                throw new AggregateException(SR.RpcEvent_OneTypeIsNotTypedEvent, exceptions);
        }

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
            return new ArrayList {
                Events.ToArray()
            };
        }

        /// <summary>Gets the events of interest.</summary>
        public IEnumerable<string> Events { get; }

        /// <summary>Gets the name of the remote method to call.</summary>
        protected override string Method => "daemon.set_event_interest";
    }
}
