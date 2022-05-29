using Ziusudra.Desktop.ViewModel;

namespace Ziusudra.Desktop.View
{

    public partial class TorrentDetailsPanel:
        UserControl
    {
        public TorrentDetailsPanel()
        {
            InitializeComponent();
        }

        public Torrent? Torrent { get; set; }
    }
}
