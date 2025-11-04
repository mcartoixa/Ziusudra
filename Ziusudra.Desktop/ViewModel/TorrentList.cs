using System.ComponentModel;

namespace Ziusudra.Desktop.ViewModel
{

    public class TorrentList:
        BindingList<Torrent>,
        IReadOnlyDictionary<string, Torrent>
    {

        public TorrentList():
            base()
        {
            AllowNew = true;
        }

        public TorrentList(IList<Torrent> torrents):
            base(torrents)
        {
            AllowNew = true;
            _InitInternalDictionary();
        }

        public bool ContainsHash(string hash)
        {
            return _TorrentsByHash.ContainsKey(hash);
        }

        public void Update(IEnumerable<Torrent> torrents)
        {
            foreach (Torrent t in torrents.Except(Items))
            {
                Remove(t);
                _TorrentsByHash.Remove(t.Hash);
            }
            foreach (Torrent t in torrents)
                if (!ContainsHash(t.Hash))
                {
                    Add(t);
                    _TorrentsByHash[t.Hash] = t;
                } else
                    this[t.Hash].Update(t);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _TorrentsByHash.Clear();
        }

        protected override void InsertItem(int index, Torrent item)
        {
            base.InsertItem(index, item);
            _TorrentsByHash.Add(item.Hash, item);
        }

        protected override void RemoveItem(int index)
        {
            _TorrentsByHash.Remove(Items[index].Hash);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, Torrent item)
        {
            _TorrentsByHash.Remove(Items[index].Hash);
            base.SetItem(index, item);
            _TorrentsByHash.Add(item.Hash, item);
        }

        private void _InitInternalDictionary()
        {
            _TorrentsByHash.Clear();
            foreach (Torrent t in Items)
                _TorrentsByHash.Add(t.Hash, t);
        }

        IEnumerator<KeyValuePair<string, Torrent>> IEnumerable<KeyValuePair<string, Torrent>>.GetEnumerator()
        {
            return _TorrentsByHash.GetEnumerator();
        }

        bool IReadOnlyDictionary<string, Torrent>.ContainsKey(string key)
        {
            return ContainsHash(key);
        }

        bool IReadOnlyDictionary<string, Torrent>.TryGetValue(string key, out Torrent value)
        {
            return _TorrentsByHash.TryGetValue(key, out value);
        }

        public IEnumerable<string> Hashes => _TorrentsByHash.Keys;

        public Torrent this[string hash] => _TorrentsByHash[hash];

        IEnumerable<string> IReadOnlyDictionary<string, Torrent>.Keys => Hashes;

        IEnumerable<Torrent> IReadOnlyDictionary<string, Torrent>.Values => _TorrentsByHash.Values;

        int IReadOnlyCollection<KeyValuePair<string, Torrent>>.Count => _TorrentsByHash.Count;

        private Dictionary<string, Torrent> _TorrentsByHash = new Dictionary<string, Torrent>();
    }
}
