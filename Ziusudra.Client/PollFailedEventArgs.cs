namespace Ziusudra.Client
{

    /// <summary>Carries the error that made a <see cref="TorrentMonitor" /> poll fail.</summary>
    public sealed class PollFailedEventArgs:
        EventArgs
    {

        /// <summary>Create a new instance of the <see cref="PollFailedEventArgs" /> type.</summary>
        /// <param name="exception">The error that made the poll fail.</param>
        public PollFailedEventArgs(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            Exception = exception;
        }

        /// <summary>Gets the error that made the poll fail.</summary>
        public Exception Exception { get; }
    }
}
