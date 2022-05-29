using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ziusudra.Desktop
{
    public partial class MainForm:
        Form
    {

        public MainForm()
        {
            InitializeComponent();

            // TODO: make this work through the designer
            _ServerVersionToolStripStatusLabel.DataBindings.Add(new Binding("Text", DelugeServerBindingSource, "Version", true));
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _Server = new ViewModel.DelugeServer(IPEndPoint.Parse("127.0.0.1:58846")) {
                    Logger = Logger
            };
            await _Server.InitAsync("deluge", "deluge");
            DelugeServerBindingSource.DataSource = _Server;
        }

        protected override async void OnClosed(EventArgs e)
        {
            if (_Server != null)
                await ((IAsyncDisposable)_Server).DisposeAsync();

            base.OnClosed(e);
        }

        /// <summary>Get or set the current logger.</summary>
        public ILogger Logger
        {
            get
            {
                return _Logger;
            }
            set
            {
                _Logger = value;
            }
        }

        private ILogger _Logger = NullLogger.Instance;
        private ViewModel.DelugeServer? _Server;
    }
}
