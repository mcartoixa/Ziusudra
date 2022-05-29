using Microsoft.Extensions.Logging;

namespace Ziusudra.Desktop
{
    internal static class Program
    {
        /// <summary>The main entry point for the application.</summary>
        [STAThread]
        static void Main()
        {
            using var loggerFactory = LoggerFactory.Create(builder => {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddDebug();
            });

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm() {
                Logger = loggerFactory.CreateLogger<MainForm>()
            });
        }
    }
}
