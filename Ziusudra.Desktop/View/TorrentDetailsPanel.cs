using System.ComponentModel;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Torrent? Torrent { get; set; }
    }
}
