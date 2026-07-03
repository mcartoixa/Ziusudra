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
        public MainWindow(ConnectionManagerViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);
            _ViewModel = viewModel;

            InitializeComponent();
            if (Content is FrameworkElement root)
                root.DataContext = viewModel;
            if (Content is FrameworkElement loadable)
                loadable.Loaded += OnRootLoaded;
        }

        private async void OnRootLoaded(object sender, RoutedEventArgs e)
        {
            await _ViewModel.InitializeAsync();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _ViewModel.Password = PasswordInput.Password;
        }

        private readonly ConnectionManagerViewModel _ViewModel;
    }
}
