using System.ComponentModel;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ziusudra.Desktop.ViewModel
{

    public class DelugeServer:
        ViewEntity,
        IAsyncDisposable
    {

        public DelugeServer()
        {
            _Client = new DelugeRpc.RpcClient(IPEndPoint.Parse("127.0.0.1:58846"));
            Username = string.Empty;
            Password = string.Empty;
        }

        public DelugeServer(IPEndPoint host, string username, string password)
        {
            _Client = new DelugeRpc.RpcClient(host);
            Username = username;
            Password = password;
        }

        public async ValueTask ConnectAsync()
        {
            _Client.RpcEventReceived += _Client_RpcEventReceived;
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.LoginRequest(Username, Password)));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetConfig()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.SetEventInterestRequest()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetSessionState()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetTorrentsStatusRequest()));
        }

        public async ValueTask InitAsync()
        {
            await _Client.StartAsync();
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.InfoRequest()));
            //HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.GetMethodList()));
        }

        public async ValueTask GetTorrentsStatus(IEnumerable<string> keys)
        {
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetTorrentsStatusRequest(keys)));
        }

        private void HandleResponse(DelugeRpc.Core.GetConfig.Response response)
        { }

        private void HandleResponse(DelugeRpc.Core.GetSessionState.Response response)
        { }

        private void HandleResponse(DelugeRpc.Core.GetTorrentsStatusRequest.Response response)
        {
            if (Torrents.Count > 0)
            {
                foreach (var t in response.Torrents)
                {
                    if (Torrents.ContainsHash(t.Hash))
                        Torrents[t.Hash].Update(t);
                    else
                        Torrents.Add(new Torrent(t));
                }

            } else
                Torrents = new TorrentList(
                    response.Torrents
                        .Select(t => new Torrent(t))
                        .ToArray()
                );
        }

        private void HandleResponse(DelugeRpc.Daemon.GetMethodList.Response response)
        {
            Methods = response.Methods;
        }

        private void HandleResponse(DelugeRpc.Daemon.InfoRequest.Response response)
        {
            Version = response.Version;
        }

        private void HandleResponse(DelugeRpc.Daemon.LoginRequest.Response response)
        { }

        private void HandleResponse(DelugeRpc.Daemon.SetEventInterestRequest.Response response)
        { }

        private void _Client_RpcEventReceived(object? sender, DelugeRpc.RpcEventReceivedEventArgs e)
        {
            Logger.LogDebug("Event received: {0}", e.Event);
        }

        private async ValueTask DisposeAsync()
        {
            await _Client.StopAsync();
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return DisposeAsync();
        }

        /// <summary>Get or set the current logger.</summary>
        [Browsable(false)]
        public ILogger Logger
        {
            get
            {
                return _Logger;
            }
            set
            {
                _Logger = value;
                _Client.Logger = value;
            }
        }

        [Browsable(false)]
        public IPEndPoint Host => _Client.Host;

        public string Hostname => _Client.Host.Address.ToString();

        [Browsable(false)]
        public IList<string> Methods
        {
            get
            {
                return _Methods ?? Array.Empty<string>();
            }
            private set
            {
                if (_Methods != value)
                {
                    _Methods = value.ToArray();
                    OnPropertyChanged();
                }
            }
        }

        public string Password { get; init; }

        public string Username { get; init; }

        public TorrentList Torrents
        {
            get
            {
                return _Torrents;
            }
            private set
            {
                if (_Torrents != value)
                {
                    _Torrents = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Version
        {
            get
            {
                return _Version ?? string.Empty;
            }
            private set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly DelugeRpc.RpcClient _Client;
        private ILogger _Logger = NullLogger.Instance;
        private string[]? _Methods;
        private TorrentList _Torrents = new TorrentList();
        private string? _Version;
    }
}
