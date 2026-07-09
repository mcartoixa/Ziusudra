using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using Ziusudra.Client;

namespace Ziusudra.Desktop
{

    /// <summary>The application shell window, hosting the connection manager and torrent list.</summary>
    public sealed partial class MainWindow:
        Window
    {

        /// <summary>Create a new instance of the <see cref="MainWindow" /> type.</summary>
        /// <param name="viewModel">The connection-manager view-model.</param>
        /// <param name="torrents">The torrent-list view-model.</param>
        /// <param name="filters">The filter-sidebar view-model.</param>
        public MainWindow(ViewModel.ConnectionManager viewModel, ViewModel.TorrentList torrents, ViewModel.FilterSidebar filters)
        {
            ArgumentNullException.ThrowIfNull(viewModel);
            ArgumentNullException.ThrowIfNull(torrents);
            ArgumentNullException.ThrowIfNull(filters);
            _ViewModel = viewModel;
            _Torrents = torrents;
            _Filters = filters;

            InitializeComponent();
            if (Content is FrameworkElement root)
            {
                root.DataContext = viewModel;
                root.Loaded += OnRootLoaded;
            }
            TorrentArea.DataContext = torrents;
        }

        private async void OnRootLoaded(object sender, RoutedEventArgs e)
        {
            await _ViewModel.InitializeAsync();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _ViewModel.Password = PasswordInput.Password;
        }

        private async void OnAddFileClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            picker.FileTypeFilter.Add(".torrent");

            StorageFile? file = await picker.PickSingleFileAsync();
            if (file is null)
                return;

            byte[] content;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            using (var reader = new DataReader(stream))
            {
                await reader.LoadAsync((uint)stream.Size);
                content = new byte[stream.Size];
                reader.ReadBytes(content);
            }

            await _Torrents.AddFileAsync(file.Name, content);
        }

        private void OnTorrentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListView)sender;
            _Torrents.SetSelection(list.SelectedItems.OfType<ViewModel.TorrentRow>().ToList());
        }

        private void OnTorrentRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var list = (ListView)sender;
            if ((e.OriginalSource as FrameworkElement)?.DataContext is not ViewModel.TorrentRow row || list.SelectedItems.Contains(row))
                return;

            list.SelectedItem = row;
        }

        private async void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<ViewModel.TorrentRow> selection = _Torrents.Selection;
            if (selection.Count == 0)
                return;

            string prompt = selection.Count == 1
                ? $"Remove “{selection[0].Name}” from the session?"
                : $"Remove {selection.Count} torrents from the session?";

            var deleteData = new CheckBox { Content = "Also delete the downloaded data from disk" };
            var body = new StackPanel { Spacing = 12 };
            body.Children.Add(new TextBlock
            {
                Text = prompt,
                TextWrapping = TextWrapping.WrapWholeWords,
            });
            body.Children.Add(deleteData);

            var dialog = new ContentDialog
            {
                Title = "Remove torrent",
                Content = body,
                PrimaryButtonText = "Remove",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = Content.XamlRoot,
            };

            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return;

            await _Torrents.RemoveSelectedAsync(deleteData.IsChecked == true);
        }

        private async void OnAddMagnetClick(object sender, RoutedEventArgs e)
        {
            var input = new TextBox { PlaceholderText = "magnet:?xt=…" };
            var dialog = new ContentDialog
            {
                Title = "Add magnet link",
                Content = input,
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Content.XamlRoot,
            };

            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return;

            await _Torrents.AddMagnetLinkAsync(input.Text);
        }

        private async void OnAboutClick(object sender, RoutedEventArgs e)
        {
            Version? version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var dialog = new ContentDialog
            {
                Title = "About Ziusudra",
                Content = new TextBlock
                {
                    Text = $"Ziusudra\nA Windows client for Deluge.\n\nVersion {version}",
                    TextWrapping = TextWrapping.WrapWholeWords,
                },
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = Content.XamlRoot,
            };

            await dialog.ShowAsync();
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void OnAddHostClick(object sender, RoutedEventArgs e)
        {
            string defaultPort = HostEntry.DefaultPort.ToString(CultureInfo.InvariantCulture);
            (string Name, string Host, string Port, string User)? input = await ShowHostDialogAsync("Add host", string.Empty, string.Empty, defaultPort, string.Empty);
            if (input is { } values)
                await _ViewModel.AddHostAsync(values.Name, values.Host, values.Port, values.User);
        }

        private async void OnEditHostClick(object sender, RoutedEventArgs e)
        {
            if (_ViewModel.SelectedHost is not ViewModel.HostRow row)
                return;

            string port = row.Entry.Port.ToString(CultureInfo.InvariantCulture);
            (string Name, string Host, string Port, string User)? input = await ShowHostDialogAsync("Edit host", row.Entry.Name, row.Entry.Host, port, row.Entry.Username);
            if (input is { } values)
                await _ViewModel.UpdateHostAsync(row, values.Name, values.Host, values.Port, values.User);
        }

        private async Task<(string Name, string Host, string Port, string User)?> ShowHostDialogAsync(string title, string name, string host, string port, string user)
        {
            var nameBox = new TextBox { Header = "Name", Text = name };
            var hostBox = new TextBox { Header = "Host", Text = host };
            var portBox = new TextBox { Header = "Port", Text = port };
            var userBox = new TextBox { Header = "User name", Text = user };
            var body = new StackPanel { Spacing = 8 };
            body.Children.Add(nameBox);
            body.Children.Add(hostBox);
            body.Children.Add(portBox);
            body.Children.Add(userBox);

            var dialog = new ContentDialog
            {
                Title = title,
                Content = body,
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Content.XamlRoot,
            };

            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return null;

            return (nameBox.Text, hostBox.Text, portBox.Text, userBox.Text);
        }

        private async void OnFilterInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is ViewModel.FilterNode node)
                await _Filters.SelectAsync(node);
        }

        private readonly ViewModel.ConnectionManager _ViewModel;
        private readonly ViewModel.TorrentList _Torrents;
        private readonly ViewModel.FilterSidebar _Filters;
    }
}
