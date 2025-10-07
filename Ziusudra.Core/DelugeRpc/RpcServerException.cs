using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Ziusudra.DelugeRpc
{

    /// <summary>An error that occurred on the server while processing a request.</summary>
    public class RpcServerException:
        RpcException,
        IServerReply
    {

        /// <summary>Create a new instance of the <see cref="RpcServerException" /> type.</summary>
        /// <param name="values">The values to create </param>
        internal RpcServerException(ICollection values):
            base(GetMessageFromValues(values))
        {
            if (values is IList l)
                Values = l;
            else
                Values = new ArrayList(values);

            Debug.Assert(((IServerMessage)this).MessageType == RpcMessageType.RPC_ERROR);
        }

        ICollection IMessage.ToValueCollection()
        {
            return Values;
        }

        /// <summary>Creates a typed error from the specified values.</summary>
        /// <param name="values">The values to create the error with.</param>
        /// <returns>The error.</returns>
        /// <remarks>A typed error is an instance of a class that inherits from <see cref="RpcServerException" />.
        /// If no specific type is found for the event then a plain <see cref="RpcServerException" /> is returned.</remarks>
        public static RpcServerException CreateFromValues(ICollection values)
        {
            RpcServerException ex = new(values);
            if (_TypedErrorsLookup.Value.TryGetValue(ex.Source ?? string.Empty, out Type? retType))
            {
                var retConstructor = retType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { typeof(ICollection) });
                if (retConstructor != null)
                    return (RpcServerException)retConstructor.Invoke(new object?[] { values });
            }

            return ex;
        }

        internal static bool IsTypedError(Type type)
        {
            return
                type.IsSubclassOf(typeof(RpcServerException)) &&
                type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { typeof(ICollection) }) != null;
        }

        private static IEnumerable<Type> InitTypedErrorsList()
        {
            return typeof(RpcServerException).Assembly.GetTypes()
                .Where(t => IsTypedError(t));
        }

        private static IDictionary<string, Type> InitTypedErrorsLookup()
        {
            return _TypedErrorsList.Value
                .ToDictionary(t => t.Name, t => t);
        }

        private static string GetMessageFromValues(ICollection values)
        {
            return values.Cast<object?>().ElementAtOrDefault(3) as string ?? string.Empty;
        }

        /// <summary>Get or sets the name of the application or the object that causes the error.</summary>
        public override string? Source => Values[2] as string;

        /// <summary>Get a string representation of the immediate frames on the call stack.</summary>
        public override string? StackTrace => Values[4] as string;

        int IExchangeMessage.Id => Convert.ToInt32(Values[1]);

        RpcMessageType IServerMessage.MessageType => (RpcMessageType)Convert.ToInt32(Values[0]);

        internal IList Values { get; }

        private static readonly Lazy<IEnumerable<Type>> _TypedErrorsList = new(InitTypedErrorsList);
        private static readonly Lazy<IDictionary<string, Type>> _TypedErrorsLookup = new(InitTypedErrorsLookup);
    }
}
