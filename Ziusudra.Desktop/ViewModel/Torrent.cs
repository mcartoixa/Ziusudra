using Ziusudra.DelugeRpc.Core;

namespace Ziusudra.Desktop.ViewModel
{

    public class Torrent:
        ViewEntity
    {

        public Torrent(DelugeRpc.Core.Torrent torrent)
        {
            _Hash = torrent.Hash;

            if (torrent.ActiveTime.HasValue)
                _ActiveTime = torrent.ActiveTime;
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
        }

        public void Update(DelugeRpc.Core.Torrent torrent)
        {
            if (torrent.ActiveTime.HasValue)
                ActiveTime = torrent.ActiveTime;
            if (torrent.Name != null)
                Name = torrent.Name;
            if (torrent.Progress.HasValue)
                Progress = torrent.Progress;
            if (torrent.Queue.HasValue)
                Queue = torrent.Queue;
            if (torrent.SeedingTime.HasValue)
                SeedingTime = torrent.SeedingTime;
            if (torrent.TotalWanted.HasValue)
                TotalWanted = torrent.TotalWanted;
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
