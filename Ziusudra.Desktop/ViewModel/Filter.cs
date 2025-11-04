namespace Ziusudra.Desktop.ViewModel
{

    public class Filter:
        ViewEntity,
        IEquatable<Filter>
    {

        public Filter(DelugeRpc.Core.Filter filter)
        {
            _Category = filter.Category;
            _Value = filter.Value;
            if (filter.Count.HasValue)
                _Count = filter.Count;
        }

        public void Update(Filter filter)
        {
            _Category = filter.Category;
            _Value = filter.Value;
            _Count = filter.Count;
        }

        public override bool Equals(object? obj) => Equals(obj as Filter);

        public bool Equals(Filter? other)
        {
            if (other is null)
                return false;
            return Category.Equals(other.Category) && Value.Equals(other.Value);
        }

        public override int GetHashCode() => Tuple.Create(Category, Value).GetHashCode();

        public static bool operator ==(Filter? torrent1, Filter? torrent2)
        {
            if (torrent1 is null)
                return torrent2 is null;
            return torrent1.Equals(torrent2);
        }

        public static bool operator !=(Filter? torrent1, Filter? torrent2)
        {
            if (torrent1 is null)
                return torrent2 is not null;
            return !torrent1.Equals(torrent2);
        }

        public string Category
        {
            get { return _Category ?? string.Empty; }
            private set
            {
                if (_Category != value)
                {
                    _Category = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Value
        {
            get { return _Value ?? string.Empty; }
            private set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? Count
        {
            get { return _Count; }
            private set
            {
                if (_Count != value)
                {
                    _Count = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Category;
        private string _Value;
        private int? _Count;
    }
}
