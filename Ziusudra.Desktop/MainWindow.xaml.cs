using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace Ziusudra.Desktop
{

    /// <summary>The application shell window, hosting the connection manager and torrent list.</summary>
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
            _Torrents = torrents;

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

        private readonly ViewModel.ConnectionManager _ViewModel;
        private readonly ViewModel.TorrentList _Torrents;
    }
}
