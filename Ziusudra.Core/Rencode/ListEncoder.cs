using System.Collections;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="ICollection" /> values.</summary>
    public sealed class ListEncoder:
        Encoder<ICollection>
    {

        internal sealed class TermEncoder:
            FixedByteEncoder
        {

            public TermEncoder() :
                base(CHR_TERM)
            { }
        }

        /// <inheritdoc />
        public override bool CanRead(byte header)
        {
            return (header == CHR_LIST) || (header >= FIXED_START && header < FIXED_START + FIXED_COUNT);
        }

        /// <inheritdoc />
        protected async override ValueTask<ICollection> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            var ret = new ArrayList();
            if (header >= FIXED_START && header < FIXED_START + FIXED_COUNT)
            {
                for (int i=0; i < header - FIXED_START; i++)
                    ret.Add(await reader.ReadValueAsync(cancellationToken).ConfigureAwait(false));
            } else if (header == CHR_LIST)
            {
                object? current = await reader.ReadValueAsync(cancellationToken, _TermEncoder).ConfigureAwait(false);
                if (current == null)
                    throw new RencodeException("Invalid data");
                while (current is not byte b || b != CHR_TERM)
                {
                    ret.Add(current);

                    current = await reader.ReadValueAsync(cancellationToken, _TermEncoder).ConfigureAwait(false);
                    if (current == null)
                        throw new RencodeException("Invalid data");
                }
            }
            return ret;
        }

        /// <inheritdoc />
        protected async override ValueTask DoWriteValueAsync(IRencodeWriter writer, ICollection value, CancellationToken cancellationToken)
        {
            await writer.WriteAsync(new byte[] { value.Count < FIXED_COUNT ? (byte)(FIXED_START + value.Count) : CHR_LIST }, cancellationToken).ConfigureAwait(false);
            foreach (var item in value)
                await writer.WriteValueAsync(item, cancellationToken).ConfigureAwait(false);
            if (value.Count >= FIXED_COUNT)
                await writer.WriteAsync(new byte[] { CHR_TERM }, cancellationToken).ConfigureAwait(false);
        }

        internal static readonly ListEncoder Instance = new();

        internal const byte CHR_LIST = 59;
        internal const byte CHR_TERM = 127;
        internal const byte FIXED_START = StringEncoder.FIXED_START + StringEncoder.FIXED_COUNT;
        internal const byte FIXED_COUNT = 25;

        private static readonly IEncoder _TermEncoder = new TermEncoder();
    }
}
