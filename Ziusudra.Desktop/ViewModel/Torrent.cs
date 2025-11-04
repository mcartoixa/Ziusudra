using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Desktop.ViewModel
{

    public class Torrent:
        ViewEntity,
        IComparable<Torrent>,
        IEquatable<Torrent>
    {

        public Torrent(DelugeRpc.Core.Torrent torrent)
        {
            _Hash = torrent.Hash;

            if (torrent.ActiveTime.HasValue)
                _ActiveTime = torrent.ActiveTime;
            if (torrent.DownloadPayloadRate.HasValue)
                _DownloadPayloadRate = torrent.DownloadPayloadRate;
            if (torrent.ExpectedTimeOfArrival.HasValue)
                ExpectedTimeOfArrival = torrent.ExpectedTimeOfArrival;
            if (torrent.Name != null)
                _Name = torrent.Name;
            if (torrent.Progress.HasValue)
                _Progress = torrent.Progress;
            if (torrent.Queue.HasValue)
                _Queue = torrent.Queue;
            if (torrent.SeedingTime.HasValue)
                _SeedingTime = torrent.SeedingTime;
            if (torrent.TotalWanted.HasValue)
                _TotalWanted = torrent.TotalWanted;
            if (torrent.UploadPayloadRate.HasValue)
                _UploadPayloadRate = torrent.UploadPayloadRate;
        }

        public void Update(Torrent torrent)
        {
            ActiveTime = torrent.ActiveTime;
            DownloadPayloadRate = torrent.DownloadPayloadRate;
            ExpectedTimeOfArrival = torrent.ExpectedTimeOfArrival;
            Name = torrent.Name;
            Progress = torrent.Progress;
            Queue = torrent.Queue;
            SeedingTime = torrent.SeedingTime;
            TotalWanted = torrent.TotalWanted;
            UploadPayloadRate = torrent.UploadPayloadRate;
        }

        public override bool Equals(object? obj) => Equals(obj as Torrent);

        public bool Equals(Torrent? other)
        {
            if (other is null)
                return false;
            return Hash.Equals(other.Hash);
        }

        public override int GetHashCode() => Hash.GetHashCode();

        int IComparable<Torrent>.CompareTo(Torrent? other)
        {
            if (other is null)
                return 1;
            return Hash.CompareTo(other.Hash);
        }

        public static bool operator ==(Torrent? torrent1, Torrent? torrent2)
        {
            if (torrent1 is null)
                return torrent2 is null;
            return torrent1.Equals(torrent2);
        }

        public static bool operator !=(Torrent? torrent1, Torrent? torrent2)
        {
            if (torrent1 is null)
                return torrent2 is not null;
            return !torrent1.Equals(torrent2);
        }

        public TimeSpan? ActiveTime
        {
            get { return _ActiveTime; }
            private set
            {
                if (_ActiveTime != value)
                {
                    _ActiveTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? DownloadPayloadRate
        {
            get { return _DownloadPayloadRate; }
            private set
            {
                if (_DownloadPayloadRate != value)
                {
                    _DownloadPayloadRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan? ExpectedTimeOfArrival
        {
            get { return _ExpectedTimeOfArrival; }
            private set
            {
                if (_ExpectedTimeOfArrival != value)
                {
                    _ExpectedTimeOfArrival = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Hash
        {
            get { return _Hash ?? string.Empty; }
            private set
            {
                if (_Hash != value)
                {
                    _Hash = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Name
        {
            get { return _Name ?? string.Empty; }
            private set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public float? Progress
        {
            get { return _Progress; }
            private set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? Queue
        {
            get { return _Queue; }
            private set
            {
                if (_Queue != value)
                {
                    _Queue = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan? SeedingTime
        {
            get { return _SeedingTime; }
            private set
            {
                if (_SeedingTime != value)
                {
                    _SeedingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public TorrentState? State
        {
            get { return _State; }
            private set
            {
                if (_State != value)
                {
                    _State = value;
                    OnPropertyChanged();
                }
            }
        }

        public long? TotalWanted
        {
            get { return _TotalWanted; }
            private set
            {
                if (_TotalWanted != value)
                {
                    _TotalWanted = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? UploadPayloadRate
        {
            get { return _UploadPayloadRate; }
            private set
            {
                if (_UploadPayloadRate != value)
                {
                    _UploadPayloadRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan? _ActiveTime;
        private int? _DownloadPayloadRate;
        private TimeSpan? _ExpectedTimeOfArrival;
        private string _Hash;
        private string? _Name;
        private float? _Progress;
        private int? _Queue;
        private TimeSpan? _SeedingTime;
        private TorrentState? _State;
        private long? _TotalWanted;
        private int? _UploadPayloadRate;
    }
}
