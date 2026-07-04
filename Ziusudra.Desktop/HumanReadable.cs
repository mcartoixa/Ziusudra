using System.Globalization;

namespace Ziusudra.Desktop
{

    /// <summary>Formats raw torrent values into short, culture-aware display strings for the UI.</summary>
    internal static class HumanReadable
    {

        /// <summary>Formats a 0-to-1 ratio as a percentage, e.g. <c>45.2 %</c>.</summary>
        /// <param name="ratio">The ratio to format.</param>
        public static string Percent(float? ratio)
        {
            return ratio is float value
                ? value.ToString("P1", CultureInfo.CurrentCulture)
                : Dash;
        }

        /// <summary>Formats a byte count, e.g. <c>1.4 GB</c>.</summary>
        /// <param name="bytes">The byte count to format.</param>
        public static string Bytes(long? bytes)
        {
            if (bytes is not long value)
                return Dash;

            double scaled = value;
            int unit = 0;
            while (scaled >= 1024 && unit < _Units.Length - 1)
            {
                scaled /= 1024;
                unit++;
            }
            return string.Format(CultureInfo.CurrentCulture, unit == 0 ? "{0:0} {1}" : "{0:0.0} {1}", scaled, _Units[unit]);
        }

        /// <summary>Formats a transfer rate, e.g. <c>1.4 MB/s</c>; a zero or absent rate reads as a dash.</summary>
        /// <param name="bytesPerSecond">The rate to format.</param>
        public static string Rate(int? bytesPerSecond)
        {
            return bytesPerSecond is int value && value > 0
                ? string.Format(CultureInfo.CurrentCulture, "{0}/s", Bytes(value))
                : Dash;
        }

        /// <summary>Formats a duration compactly, e.g. <c>1h 23m</c>; a zero or absent duration reads as a dash.</summary>
        /// <param name="duration">The duration to format.</param>
        public static string Duration(TimeSpan? duration)
        {
            if (duration is not TimeSpan value || value <= TimeSpan.Zero)
                return Dash;

            if (value.TotalDays >= 1)
                return string.Format(CultureInfo.CurrentCulture, "{0}d {1}h", (int)value.TotalDays, value.Hours);
            if (value.TotalHours >= 1)
                return string.Format(CultureInfo.CurrentCulture, "{0}h {1}m", value.Hours, value.Minutes);
            if (value.TotalMinutes >= 1)
                return string.Format(CultureInfo.CurrentCulture, "{0}m {1}s", value.Minutes, value.Seconds);
            return string.Format(CultureInfo.CurrentCulture, "{0}s", value.Seconds);
        }

        private const string Dash = "—";

        private static readonly string[] _Units = { "B", "KB", "MB", "GB", "TB" };
    }
}
