using System.Collections;

namespace Ziusudra.DelugeRpc.Errors
{

    /// <summary>An authorization error.</summary>
    public sealed class NotAuthorizedError:
        RpcServerException
    {

        /// <summary>Create a new instance of the <see cref="NotAuthorizedError" /> type.</summary>
        /// <param name="values">The values to create </param>
        internal NotAuthorizedError(ICollection values):
            base(values)
        { }
    }
}
