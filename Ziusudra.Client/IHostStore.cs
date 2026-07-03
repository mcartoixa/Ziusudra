namespace Ziusudra.Client
{

    /// <summary>Persists the list of saved host entries in the client's own format.</summary>
    public interface IHostStore
    {

        /// <summary>Loads the saved host entries.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The saved entries, or an empty list when none have been saved.</returns>
        Task<IReadOnlyList<HostEntry>> LoadAsync(CancellationToken cancellationToken = default);

        /// <summary>Saves the specified host <paramref name="entries" />, replacing any previously saved list.</summary>
        /// <param name="entries">The entries to save.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task SaveAsync(IEnumerable<HostEntry> entries, CancellationToken cancellationToken = default);
    }
}
