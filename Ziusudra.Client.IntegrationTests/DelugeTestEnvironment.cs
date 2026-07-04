using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Ziusudra.DelugeRpc;

namespace Ziusudra.Client.IntegrationTests
{

    /// <summary>Resolves the Deluge daemon endpoint and credentials for the integration tests from the environment.</summary>
    /// <remarks>Values come from environment variables, optionally injected from a local, git-ignored <c>.env</c> file
    /// (see <c>.env.example</c>). When they are absent the tests skip rather than fail, so the suite is a no-op for anyone
    /// without a server and stays out of CI.</remarks>
    internal static class DelugeTestEnvironment
    {

        static DelugeTestEnvironment()
        {
            LoadDotEnv();
        }

        public const string SkipReason =
            "Set ZIUSUDRA_DELUGE_HOST / ZIUSUDRA_DELUGE_USERNAME / ZIUSUDRA_DELUGE_PASSWORD (via the environment or a local .env) to run the Deluge integration tests.";

        public static bool IsConfigured =>
            Host is not null && Username is not null && Password is not null;

        public static async Task<DelugeSession> ConnectAsync()
        {
            IPEndPoint endpoint = await new DnsHostResolver().ResolveAsync(Host!, Port);
            var session = new DelugeSession(resolved => new RpcClient(resolved));
            await session.ConnectAsync(endpoint, Username!, Password!);
            return session;
        }

        private static string? Host => Value("ZIUSUDRA_DELUGE_HOST");
        private static string? Username => Value("ZIUSUDRA_DELUGE_USERNAME");
        private static string? Password => Value("ZIUSUDRA_DELUGE_PASSWORD");

        private static int Port =>
            int.TryParse(Value("ZIUSUDRA_DELUGE_PORT"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int port)
                ? port
                : HostEntry.DefaultPort;

        private static string? Value(string name)
        {
            string? value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static void LoadDotEnv()
        {
            // Walk up from the test output directory so a .env at the project or repository root is picked up.
            for (DirectoryInfo? directory = new(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
            {
                string candidate = Path.Combine(directory.FullName, ".env");
                if (File.Exists(candidate))
                {
                    DotNetEnv.Env.Load(candidate);
                    return;
                }
            }
        }
    }
}
