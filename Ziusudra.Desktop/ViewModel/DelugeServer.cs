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
        }

        public DelugeServer(IPEndPoint host)
        {
            _Client = new DelugeRpc.RpcClient(host);
        }

        public async ValueTask InitAsync(string username, string password)
        {
            await _Client.StartAsync();
            _Client.RpcEventReceived += _Client_RpcEventReceived;
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.InfoRequest()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.LoginRequest(username, password)));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.GetMethodList()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetConfig()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Daemon.SetEventInterestRequest()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetSessionState()));
            HandleResponse(await _Client.SendRequestAsync(new DelugeRpc.Core.GetTorrentsStatusRequest()));
        }

        private void HandleResponse(DelugeRpc.Core.GetConfig.Response response)
        { }

        private void HandleResponse(DelugeRpc.Core.GetSessionState.Response response)
        { }

        private void HandleResponse(DelugeRpc.Core.GetTorrentsStatusRequest.Response response)
        {
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

        public BindingList<Torrent> Torrents
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
        private BindingList<Torrent> _Torrents = new TorrentList();
        private string? _Version;
    }
}
