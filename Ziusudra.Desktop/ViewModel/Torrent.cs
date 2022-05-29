namespace Ziusudra.Desktop.ViewModel
{

    public class Torrent:
        ViewEntity
    {

        public Torrent(DelugeRpc.Core.Torrent torrent)
        {
            ActiveTime = torrent.ActiveTime;
            Hash = torrent.Hash;
            Name = torrent.Name;
            Progress = torrent.Progress;
            SeedingTime = torrent.SeedingTime;
        }
        public TimeSpan ActiveTime
        {
            get
            {
                return _ActiveTime;
            }
            private set
            {
                if (_ActiveTime != value)
                {
                    _ActiveTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Hash
        {
            get
            {
                return _Hash ?? string.Empty;
            }
            private set
            {
                if (_Hash != value)
                {
                    _Hash = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return _Name ?? string.Empty;
            }
            private set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public float Progress
        {
            get
            {
                return _Progress;
            }
            private set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan SeedingTime
        {
            get
            {
                return _SeedingTime;
            }
            private set
            {
                if (_SeedingTime != value)
                {
                    _SeedingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _ActiveTime;
        private string _Hash = string.Empty;
        private string _Name = string.Empty;
        private float _Progress;
        private TimeSpan _SeedingTime;
    }
}
