using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            _Window = new MainWindow();
            _Window.Activate();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Session layer: the production factory creates a real RpcClient for a resolved endpoint.
            services.AddSingleton<Func<IPEndPoint, IRpcClient>>(_ => endpoint => new RpcClient(endpoint));
            services.AddSingleton<DelugeSession>();
        }

        /// <summary>Gets the service provider for the running application.</summary>
        public IServiceProvider Services => _Host.Services;

        private readonly IHost _Host;
        private Window? _Window;
    }
}
