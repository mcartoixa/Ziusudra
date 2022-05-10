using System.Collections;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Represents a message in the Deluge RPC protocol.</summary>
    public interface IMessage
    {

        /// <summary>Transforms the current message in the actual collection of objects that will be encoded.</summary>
        /// <returns>The collection of objects that will be encoded.</returns>
        ICollection ToValueCollection();
    }
}
