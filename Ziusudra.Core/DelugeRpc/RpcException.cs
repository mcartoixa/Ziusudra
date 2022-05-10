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
    }
}
