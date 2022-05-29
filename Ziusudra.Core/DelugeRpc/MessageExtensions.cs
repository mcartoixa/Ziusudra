using System.Collections;
using System.Globalization;
using System.Text;

namespace Ziusudra.DelugeRpc
{

    /// <summary>Useful extensions for <see cref="IMessage" /> types.</summary>
    public static class MessageExtensions
    {

        /// <summary>Return a string representation of the specified <paramref name="message" />.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The string representation of the specified <paramref name="message" />.</returns>
        public static string ToDebugString(this IMessage message)
        {
            return ToDebugString(message.ToValueCollection());
        }

        private static string ToDebugString(ICollection collection)
        {
            StringBuilder sb = new();
            sb.Append('[');
            foreach(var item in collection)
            {
                if (sb.Length > 1)
                    sb.Append(", ");
                sb.Append(ToDebugString(item));
            }
            sb.Append(']');
            return sb.ToString();
        }

        private static string ToDebugString(IDictionary dictionary)
        {
            StringBuilder sb = new();
            sb.Append('{');
            foreach (DictionaryEntry entry in dictionary)
            {
                if (sb.Length > 1)
                    sb.Append(", ");
                sb.Append(ToDebugString(entry.Key));
                sb.Append(": ");
                sb.Append(ToDebugString(entry.Value));
            }
            sb.Append('}');
            return sb.ToString();
        }

        private static string ToDebugString(string @string)
        {
            return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", @string);
        }

        internal static string ToDebugString(object? @object)
        {
            if (@object == null)
                return "<null>";
            else if (@object is string str)
                return ToDebugString(str);
            // *Must* appear before ICollection, as IDictionary is also an ICollection
            else if (@object is IDictionary dic)
                return ToDebugString(dic);
            else if (@object is ICollection col)
                return ToDebugString(col);
            else
                return string.Format(CultureInfo.InvariantCulture, "{0}", @object ?? "<null>");
        }
    }
}
