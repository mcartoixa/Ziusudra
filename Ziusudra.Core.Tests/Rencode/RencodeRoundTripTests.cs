using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ziusudra.Rencode.Tests
{

    public class RencodeRoundTripTests
    {

        [Theory]
        [InlineData(0L)]
        [InlineData(43L)]           // Last positive fixed integer.
        [InlineData(-1L)]           // First negative fixed integer.
        [InlineData(-5L)]
        [InlineData(-32L)]          // Last negative fixed integer.
        [InlineData(-33L)]          // First value below the negative fixed range.
        [InlineData(44L)]           // First value above the positive fixed range.
        [InlineData(sbyte.MinValue)]
        [InlineData(sbyte.MaxValue)]
        [InlineData(128L)]
        [InlineData(-129L)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(32768L)]
        [InlineData(-32769L)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(2147483648L)]
        [InlineData(-2147483649L)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public async Task Integer_ShouldRoundTripToSameValue(long value)
        {
            object? result = await RoundTripAsync(value);

            Assert.Equal(value, Convert.ToInt64(result, CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, 0L)]
        [InlineData(new byte[] { 0x2B }, 43L)]      // Last positive fixed integer.
        [InlineData(new byte[] { 0x46 }, -1L)]      // First negative fixed integer.
        [InlineData(new byte[] { 0x4A }, -5L)]
        [InlineData(new byte[] { 0x65 }, -32L)]     // Last negative fixed integer.
        public async Task Integer_ShouldDecodeFixedFromKnownBytes(byte[] encoded, long expected)
        {
            using var reader = new RencodeStreamReader(encoded);

            object? result = await reader.ReadValueAsync(CancellationToken.None);

            Assert.Equal(expected, Convert.ToInt64(result, CultureInfo.InvariantCulture));
        }

        [Fact]
        public async Task BigInteger_ShouldRoundTripToSameValue()
        {
            BigInteger value = BigInteger.Pow(2, 128) + 1;

            object? result = await RoundTripAsync(value);

            Assert.Equal(value, Assert.IsType<BigInteger>(result));
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("é")]                           // Two UTF-8 bytes for a single character.
        [InlineData("aé")]
        [InlineData("日本語")]
        public async Task String_ShouldRoundTripToSameValue(string value)
        {
            object? result = await RoundTripAsync(value);

            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(63, 'a')]                        // Longest fixed-length ASCII string.
        [InlineData(64, 'a')]                        // First length-prefixed ASCII string.
        [InlineData(40, 'é')]                        // 40 characters but 80 bytes: char count fits the fixed range, byte count does not.
        public async Task String_ShouldRoundTripAcrossTheFixedLengthBoundary(int length, char character)
        {
            string value = new(character, length);

            object? result = await RoundTripAsync(value);

            Assert.Equal(value, result);
        }

        [Fact]
        public async Task FixedString_ShouldDecodeMultiByteCharacterFromKnownBytes()
        {
            // 0x82 is the fixed-string header for a two-byte payload, followed by the UTF-8 bytes of 'é'.
            using var reader = new RencodeStreamReader(new byte[] { 0x82, 0xC3, 0xA9 });

            object? result = await reader.ReadValueAsync(CancellationToken.None);

            Assert.Equal("é", result);
        }

        private static async Task<object?> RoundTripAsync(object? value)
        {
            using var ms = new MemoryStream();
            using (var writer = new RencodeStreamWriter(ms, true))
                await writer.WriteValueAsync(value, CancellationToken.None);

            ms.Position = 0;
            using var reader = new RencodeStreamReader(ms, true);
            return await reader.ReadValueAsync(CancellationToken.None);
        }
    }
}
