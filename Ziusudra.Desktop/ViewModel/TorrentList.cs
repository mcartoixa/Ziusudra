using System.ComponentModel;

namespace Ziusudra.Desktop.ViewModel
{

    public class TorrentList:
        BindingList<Torrent>
    {

        public TorrentList():
            base()
        { }

        public TorrentList(IList<Torrent> torrents):
            base(torrents)
        { }
    }
}
