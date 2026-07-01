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
        /// <param name="message">The message of the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RpcException(string message, Exception innerException):
            base(message, innerException)
        { }
    }
}
