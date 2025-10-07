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

            _DownloadPayloadRateDataGridViewTextBoxColumn.Tag = "download_payload_rate";
            _ExpectedTimeOfArrivalDataGridViewTextBoxColumn.Tag = "eta";
            _NameDataGridViewTextBoxColumn.Tag = "state,name";
            _ProgressDataGridViewTextBoxColumn.Tag = "progress,state";
            _QueueDataGridViewTextBoxColumn.Tag = "queue";
            _TotalWantedDataGridViewTextBoxColumn.Tag = "total_wanted";
            _UploadPayloadRateDataGridViewTextBoxColumn.Tag = "upload_payload_rate";
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            await _Server.InitAsync();
            await _Server.ConnectAsync();
            DelugeServerBindingSource.DataSource = _Server;
            _RefreshTimer.Enabled = true;
        }

        protected override async void OnClosed(EventArgs e)
        {
            _RefreshTimer.Enabled = false;
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

        private void _ConnectionManagerToolStripButton_Click(object sender, EventArgs e)
        {
            using var form = new ConnectionManagerForm();
            form.ShowDialog(this);
        }

        private async void _RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_Server != null)
            {
                var keys = _TorrentsView.Columns.Cast<DataGridViewColumn>()
                    .Where(dvgc => dvgc.Visible)
                    .Select(dgvc => dgvc.Tag)
                    .OfType<string>()
                    .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .Distinct();
                await _Server.GetTorrentsStatus(keys);
            }
        }

        private ILogger _Logger = NullLogger.Instance;
        private ViewModel.DelugeServer? _Server;
    }
}
