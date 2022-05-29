using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="string" /> values.</summary>
    public sealed class StringEncoder:
        Encoder<string>
    {

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return
                ((header >= (byte)'0') && (header <= (byte)'9')) ||
                ((header >= FIXED_START) && (header < FIXED_START + FIXED_COUNT));
        }

        /// <inheritdoc />
        protected async override ValueTask<string> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            int stringLength;
            if (header < FIXED_START)
            {
                // Length is prefixed as a string terminated by ':'
                char current = (char)header;
                StringBuilder sb = new (10);
                while (current != ':')
                {
                    sb.Append(current);
                    current = (char)(await reader.ReadAsync(1, cancellationToken).ConfigureAwait(false))[0];
                }
                stringLength = int.Parse(sb.ToString(), CultureInfo.InvariantCulture);
            } else
            {
                stringLength = header - FIXED_START;
            }
            if (stringLength == 0)
                return string.Empty;

            byte[] buffer = await reader.ReadAsync(stringLength, cancellationToken)
                .ConfigureAwait(false);
            using var ms = new MemoryStream(buffer, false);
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            {
                char[] sb = new char[stringLength];
                int read = sr.Read(sb);
                return new string(sb, 0, read);
            }
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, string value, CancellationToken cancellationToken)
        {
            byte[] content = Encoding.UTF8.GetBytes(value);

            byte[] header = value.Length < FIXED_COUNT ?
                new byte[] { (byte)(FIXED_START + value.Length) } :
                Encoding.ASCII.GetBytes(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0:D}:",
                        content.Length
                    )
                );
            await writer.WriteAsync(header, cancellationToken)
                .ConfigureAwait(false);

            if (!string.IsNullOrEmpty(value))
                await writer.WriteAsync(content, cancellationToken)
                    .ConfigureAwait(false);
        }

        internal static readonly StringEncoder Instance = new();

        internal const int FIXED_COUNT = 64;
        internal const byte FIXED_START = 128;
    }
}
