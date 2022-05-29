using System.Collections;
using System.Reflection;

namespace Ziusudra.DelugeRpc
{

    /// <summary>An event sent by the server.</summary>
    public class RpcEvent:
        IServerMessage
    {

        /// <summary>Create a new instance of the <see cref="RpcEvent" /> type.</summary>
        /// <param name="values">The values to create the event with.</param>
        protected RpcEvent(ICollection values)
        {
            if (values is IList l)
                Values = l;
            else
                Values = new ArrayList(values);
        }

        ICollection IMessage.ToValueCollection()
        {
            return Values;
        }

        /// <summary>Creates a typed event from the specified values.</summary>
        /// <param name="values">The values to create the event with.</param>
        /// <returns>The event.</returns>
        /// <remarks>A typed event is an instance of a class that inherits from <see cref="RpcEvent" />.
        /// If no specific type is found for the event then a plain <see cref="RpcEvent" /> is returned.</remarks>
        public static RpcEvent CreateFromValues(ICollection values)
        {
            RpcEvent @event = new(values);
            if (_TypedEventsLookup.Value.TryGetValue(@event.EventName, out Type? retType))
            {
                var retConstructor = retType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { typeof(ICollection) });
                if (retConstructor != null)
                    return (RpcEvent)retConstructor.Invoke(new object?[] { values });
            }

            return @event;
        }

        internal static bool IsTypedEvent(Type type)
        {
            return
                type.IsSubclassOf(typeof(RpcEvent)) &&
                type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { typeof(ICollection) }) != null;
        }

        private static IEnumerable<Type> InitTypedEventsList()
        {
            return typeof(RpcEvent).Assembly.GetTypes()
                .Where(t => IsTypedEvent(t));
        }

        private static IDictionary<string, Type> InitTypedEventsLookup()
        {
            return _TypedEventsList.Value
                .ToDictionary(t => t.Name, t => t);
        }

        internal static IEnumerable<Type> TypedEventsList { get => _TypedEventsList.Value; }

        /// <summary>Gets the data related to the event.</summary>
        protected IList Data => (Values.Count >= 3 ? Values[2] as IList : null) ?? Array.Empty<object>();

        /// <summary>The name of the event being emitted by the daemon.</summary>
        protected string EventName => Values[1]?.ToString() ?? string.Empty;

        /// <summary>Gets the values of the event.</summary>
        protected IList Values { get; }

        RpcMessageType IServerMessage.MessageType => (RpcMessageType)Convert.ToInt32(Values[0]);

        private static readonly Lazy<IEnumerable<Type>> _TypedEventsList = new(InitTypedEventsList);
        private static readonly Lazy<IDictionary<string, Type>> _TypedEventsLookup = new(InitTypedEventsLookup);
    }
}
