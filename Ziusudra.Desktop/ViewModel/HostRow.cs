using CommunityToolkit.Mvvm.ComponentModel;
using Ziusudra.Client;

namespace Ziusudra.Desktop.ViewModel
{

    /// <summary>Presents one saved host for the host list. The underlying <see cref="HostEntry" /> is a plain persistence
    /// model, so this observable wrapper is what the list binds to and what refreshes when the entry is edited.</summary>
    public sealed partial class HostRow:
        ObservableObject
    {

        /// <summary>Create a new instance of the <see cref="HostRow" /> type.</summary>
        /// <param name="entry">The host entry to present.</param>
        public HostRow(HostEntry entry)
        {
            ArgumentNullException.ThrowIfNull(entry);
            Entry = entry;
        }

        /// <summary>Gets the underlying host entry.</summary>
        public HostEntry Entry { get; }

        /// <summary>Gets the display name of the host.</summary>
        public string Name => Entry.Name;

        /// <summary>Gets the host and port of the daemon.</summary>
        public string Host => Entry.Host;

        /// <summary>The probed reachability of the host.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusDisplay))]
        private HostStatus _Status;

        /// <summary>The daemon version reported by the last probe, or empty when unknown.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusDisplay))]
        private string _Version = string.Empty;

        /// <summary>Gets the status line shown under the host, combining the status with the daemon version when known.</summary>
        public string StatusDisplay
        {
            get
            {
                string status = Status switch {
                    HostStatus.Connected => "Connected",
                    HostStatus.Online => "Online",
                    HostStatus.Offline => "Offline",
                    _ => "Checking…"
                };
                return string.IsNullOrEmpty(Version) ? status : $"{status} · Deluge {Version}";
            }
        }

        /// <summary>Records the latest probed status and version.</summary>
        /// <param name="status">The probed status.</param>
        /// <param name="version">The reported daemon version.</param>
        public void SetStatus(HostStatus status, string version)
        {
            Status = status;
            Version = version;
        }

        /// <summary>Raises change notifications after the underlying <see cref="Entry" /> has been edited.</summary>
        public void Refresh()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Host));
        }
    }
}
