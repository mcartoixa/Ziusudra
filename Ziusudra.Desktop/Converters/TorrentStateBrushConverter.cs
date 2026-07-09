using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Ziusudra.Desktop.Converters
{

    /// <summary>Maps a torrent state name to the colour of its status dot in the list.</summary>
    public sealed class TorrentStateBrushConverter:
        IValueConverter
    {

        /// <summary>Converts a torrent state name to a brush for its status dot.</summary>
        /// <param name="value">The torrent state name.</param>
        /// <param name="targetType">The target binding type.</param>
        /// <param name="parameter">The converter parameter, unused.</param>
        /// <param name="language">The binding language, unused.</param>
        /// <returns>A brush coloured by the state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (value as string) switch {
                "Downloading" => Colors.SteelBlue,
                "Seeding" => Colors.MediumSeaGreen,
                "Paused" => Colors.Gray,
                "Error" => Colors.Crimson,
                "Queued" => Colors.Goldenrod,
                "Checking" => Colors.CornflowerBlue,
                "Allocating" => Colors.CornflowerBlue,
                "Moving" => Colors.CornflowerBlue,
                _ => Colors.Gray
            };
            return new SolidColorBrush(color);
        }

        /// <summary>Not supported: the status dot is display-only.</summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
