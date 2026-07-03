using System.Text.Json;

namespace Ziusudra.Client
{

    /// <summary>Persists host entries as a JSON file at a caller-supplied path.</summary>
    public sealed class JsonHostStore:
        IHostStore
    {

        /// <summary>Create a new instance of the <see cref="JsonHostStore" /> type.</summary>
        /// <param name="filePath">The path of the JSON file backing the store.</param>
        public JsonHostStore(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath);
            _FilePath = filePath;
        }

        /// <summary>Loads the saved host entries.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The saved entries, or an empty list when the file does not exist.</returns>
        public async Task<IReadOnlyList<HostEntry>> LoadAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_FilePath))
                return Array.Empty<HostEntry>();

            using FileStream stream = File.OpenRead(_FilePath);
            List<HostEntry>? entries = await JsonSerializer.DeserializeAsync<List<HostEntry>>(stream, _JsonOptions, cancellationToken)
                .ConfigureAwait(false);
            return entries ?? (IReadOnlyList<HostEntry>)Array.Empty<HostEntry>();
        }

        /// <summary>Saves the specified host <paramref name="entries" />, replacing any previously saved list.</summary>
        /// <param name="entries">The entries to save.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SaveAsync(IEnumerable<HostEntry> entries, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entries);

            string? directory = Path.GetDirectoryName(_FilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            using FileStream stream = File.Create(_FilePath);
            await JsonSerializer.SerializeAsync(stream, entries.ToArray(), _JsonOptions, cancellationToken)
                .ConfigureAwait(false);
        }

        private static readonly JsonSerializerOptions _JsonOptions = new() {
            WriteIndented = true
        };

        private readonly string _FilePath;
    }
}
