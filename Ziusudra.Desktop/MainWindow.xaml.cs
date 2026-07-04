using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Ziusudra.Desktop
{

    /// <summary>The application shell window, hosting the connection manager.</summary>
    public sealed partial class MainWindow:
        Window
    {

        /// <summary>Create a new instance of the <see cref="MainWindow" /> type.</summary>
        /// <param name="viewModel">The connection-manager view-model.</param>
        /// <param name="torrents">The torrent-list view-model.</param>
        public MainWindow(ViewModel.ConnectionManager viewModel, ViewModel.TorrentList torrents)
        {
            ArgumentNullException.ThrowIfNull(viewModel);
            ArgumentNullException.ThrowIfNull(torrents);
            _ViewModel = viewModel;

            InitializeComponent();
            if (Content is FrameworkElement root)
            {
                root.DataContext = viewModel;
                root.Loaded += OnRootLoaded;
            }
            TorrentListView.DataContext = torrents;
        }

        private async void OnRootLoaded(object sender, RoutedEventArgs e)
        {
            await _ViewModel.InitializeAsync();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _ViewModel.Password = PasswordInput.Password;
        }

        private readonly ViewModel.ConnectionManager _ViewModel;
    }
}
