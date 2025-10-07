using System.Net;

namespace Ziusudra.Desktop.ViewModel
{

    public class DelugeServerEditData:
        ViewEntity
    {

        public DelugeServerEditData(DelugeServer server)
        {
            _HostName = server.Host.Address.ToString();
            _Port = server.Host.Port;
        }

        public DelugeServerEditData()
        {
        }

        public DelugeServer CreateServer()
        {
            return new DelugeServer(
                new IPEndPoint(
                    IPAddress.Parse(HostName),
                    Port
                ),
                Username,
                Password
            );
        }

        protected override string Error
        {
            get
            {
                try
                {
                    CreateServer();
                    return string.Empty;
                } catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string HostName
        {
            get
            {
                return _HostName;
            }
            set
            {
                if (_HostName != value)
                {
                    _HostName = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                if (_Port != value)
                {
                    _Port = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                if (_Username != value)
                {
                    _Username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _HostName = string.Empty;
        private int _Port = 58846;
        private string _Username = string.Empty;
        private string _Password = string.Empty;
    }
}
