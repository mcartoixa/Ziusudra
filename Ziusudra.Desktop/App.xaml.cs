using System.IO;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Ziusudra.Client;
using Ziusudra.DelugeRpc;

namespace Ziusudra.Desktop
{

    /// <summary>The application composition root: builds the DI host and shows the shell window.</summary>
    public partial class App:
        Application
    {

        /// <summary>Create a new instance of the <see cref="App" /> type.</summary>
        public App()
        {
            InitializeComponent();

            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            ConfigureServices(builder.Services);
            _Host = builder.Build();
        }

        /// <summary>Invoked when the application is launched.</summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _Window = Services.GetRequiredService<MainWindow>();
            _Window.Activate();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Session layer: the production factory creates a real RpcClient for a resolved endpoint.
            services.AddSingleton<Func<IPEndPoint, IRpcClient>>(_ => endpoint => new RpcClient(endpoint));
            services.AddSingleton<DelugeSession>();

            // Connection manager: persistence, host resolution, and the UI-thread marshalling seam.
            services.AddSingleton<IUIDispatcher>(_ => new UIDispatcher(DispatcherQueue.GetForCurrentThread()));
            services.AddSingleton<IHostStore>(_ => new JsonHostStore(HostStorePath));
            services.AddSingleton<IHostResolver, DnsHostResolver>();
            services.AddSingleton<ConnectionManager>();
            services.AddSingleton<ViewModel.ConnectionManager>();
            services.AddSingleton<ViewModel.TorrentList>();
            services.AddSingleton<ViewModel.FilterSidebar>();

            services.AddTransient<MainWindow>();
        }

        private static string HostStorePath =>
#if DEBUG
            // Keep the store next to the executable during development: easy to find and clean.
            Path.Combine(AppContext.BaseDirectory, "hosts.json");
#else
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ziusudra", "hosts.json");
#endif

        /// <summary>Gets the service provider for the running application.</summary>
        public IServiceProvider Services => _Host.Services;

        private readonly IHost _Host;
        private Window? _Window;
    }
}
