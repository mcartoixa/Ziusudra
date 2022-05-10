using System.Diagnostics;

namespace Ziusudra.Rencode
{

    /// <summary>Encoder dedicated to <see cref="bool" /> values.</summary>
    public sealed class NullEncoder:
        Encoder
    {

        /// <summary>Indicate whether the current <see cref="NullEncoder" /> can read the next value according to the specified <paramref name="header" />.</summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if the specified <paramref name="header" /> encodes the <c>null</c> value; or else <c>false</c>.</returns>
        public override bool CanRead(byte header)
        {
            return header == CHR_NONE;
        }

        /// <summary>Indicate whether the current <see cref="NullEncoder" /> can write the specified <paramref name="value" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified <paramref name="value" /> is <c>null</c>; or else <c>false</c>.</returns>
        public override bool CanWrite(object? value)
        {
            return value is null;
        }

        /// <summary>Read a value from the current context of the specified <paramref name="reader" />.</summary>
        /// <param name="reader">The Rencode reader to read the value from.</param>
        /// <param name="header">The current header in the Rencode serialization.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The value that has been read.</returns>
        protected override ValueTask<object?> DoReadValueAsync(IRencodeReader reader, byte header, CancellationToken cancellationToken)
        {
            Debug.Assert(header == CHR_NONE);
            return ValueTask.FromResult<object?>(null);
        }

        /// <summary>Write the specified <paramref name="value" /> to the specified <paramref name="writer" />.</summary>
        /// <param name="writer">The Rencode writer to write the </param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        protected override ValueTask DoWriteValueAsync(IRencodeWriter writer, object? value, CancellationToken cancellationToken)
        {
            Debug.Assert(value == null);
            return writer.WriteAsync(new byte[] { CHR_NONE }, cancellationToken);
        }

        internal static readonly NullEncoder Instance = new();

        private const byte CHR_NONE = 69;
    }
}
