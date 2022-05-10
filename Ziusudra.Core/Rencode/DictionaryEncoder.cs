using System.Collections;
using System.Collections.Specialized;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="IDictionary" /> values.</summary>
    public sealed class DictionaryEncoder:
        Encoder<IDictionary>
    {

        internal sealed class TermEncoder:
            FixedByteEncoder
        {

            public TermEncoder():
                base(CHR_TERM)
            { }
        }

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return (header == CHR_DICT) || (header >= FIXED_START && header < FIXED_START + FIXED_COUNT);
        }

        /// <inheritdoc />
        protected async override ValueTask<IDictionary> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            var ret = new ListDictionary();
            if (header >= FIXED_START && header < FIXED_START + FIXED_COUNT)
            {
                for (int i = 0; i < header - FIXED_START; i++)
                {
                    var key = await reader.ReadValueAsync(cancellationToken).ConfigureAwait(false);
                    if (key == null)
                        throw new RencodeException("Invalid data");
                    var value = await reader.ReadValueAsync(cancellationToken).ConfigureAwait(false);
                    ret.Add(key, value);
                }
            } else if (header == CHR_DICT)
            {
                object? key = await reader.ReadValueAsync(cancellationToken, _TermEncoder).ConfigureAwait(false);
                if (key == null)
                    throw new RencodeException("Invalid data");
                while (key is not byte b || b != CHR_TERM)
                {
                    var value = await reader.ReadValueAsync(cancellationToken).ConfigureAwait(false);
                    ret.Add(key, value);

                    key = await reader.ReadValueAsync(cancellationToken, _TermEncoder).ConfigureAwait(false);
                    if (key == null)
                        throw new RencodeException("Invalid data");
                }
            }
            return ret;
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, IDictionary value, CancellationToken cancellationToken)
        {
            await writer.WriteAsync(new byte[] { value.Count < FIXED_COUNT ? (byte)(FIXED_START + value.Count) : CHR_DICT }, cancellationToken).ConfigureAwait(false);
            foreach (DictionaryEntry entry in value)
            {
                await writer.WriteValueAsync(entry.Key, cancellationToken).ConfigureAwait(false);
                await writer.WriteValueAsync(entry.Value, cancellationToken).ConfigureAwait(false);
            }
            if (value.Count >= FIXED_COUNT)
                await writer.WriteAsync(new byte[] { CHR_TERM }, cancellationToken).ConfigureAwait(false);
        }

        internal static readonly DictionaryEncoder Instance = new();

        internal const byte CHR_DICT = 60;
        internal const byte CHR_TERM = 127;
        internal const byte FIXED_START = 102;
        internal const byte FIXED_COUNT = 25;

        private static readonly IEncoder _TermEncoder = new TermEncoder();
    }
}
