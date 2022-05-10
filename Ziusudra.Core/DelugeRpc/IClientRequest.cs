using System.Collections;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Represents a request from a client.</summary>
    public interface IClientRequest:
        IExchangeMessage
    {

        /// <summary>Gets the arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The arguments.</returns>
        ICollection GetArgs();

        /// <summary>Gets the keyword arguments to call the <see cref="Method" /> with.</summary>
        /// <returns>The keyword arguments.</returns>
        IDictionary GetKeywordArgs();

        /// <summary>Create the response from the specified values.</summary>
        /// <param name="values">The values to create the response with.</param>
        /// <returns>The response.</returns>
        IServerReply CreateResponse(ICollection values);

        /// <summary>Gets the name of the remote method to call.</summary>
        string Method { get; }
    }
}
