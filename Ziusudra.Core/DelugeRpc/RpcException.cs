using System.Runtime.Serialization;

namespace Ziusudra.DelugeRpc
{

    /// <summary>An exception related to the Deluge RPC protocol.</summary>
    public class RpcException:
        Exception
    {

        /// <summary>Creates a new instance of the <see cref="RpcException" /> type.</summary>
        /// <param name="message">The message of the exception.</param>
        public RpcException(string message):
            base(message)
        { }

        /// <summary>Creates a new instance of the <see cref="RpcException" /> type.</summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected RpcException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}
