namespace Ziusudra.Rencode
{

    /// <summary>Handles a Rencode related exception.</summary>
    public sealed class RencodeException:
        Exception
    {

        /// <summary>Create a new instance of the <see cref="RencodeException" /> type.</summary>
        /// <param name="message">The message of the exception.</param>
        public RencodeException(string message):
            base(message)
        { }
    }
}
