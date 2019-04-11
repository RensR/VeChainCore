using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Nethereum.RLP;
using Org.BouncyCastle.Math;
using VeChainCore.Models.Extensions;

namespace VeChainCore.Utils
{
    [Flags]
    public enum StringType
    {
        Default = 0,
        ZeroLowerX = 1 << 0,
        Hex = 1 << 0
    }

    public static class Hex
    {
        public static BigInteger HexToBigInt(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return new BigInteger(hex, 16);
        }

        public static decimal ToHumanReadable(decimal dec)
        {
            return dec / 1_000_000_000_000_000_000;
        }

        public static byte[] BigIntegerToBytes(this BigInteger bigInt)
        {
            return bigInt.ToByteArray().TrimLeadingZeroBytes();
        }

        public static string ByteArrayToString(this byte[] ba, StringType type = StringType.Hex)
        {
            if (!type.HasFlag(StringType.Hex))
                return Encoding.UTF8.GetString(ba);

            var hex = new StringBuilder(ba.Length * 2 + 2);

            if (type.HasFlag(StringType.ZeroLowerX))
                hex.Append("0x");

            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }

        public static byte[] StringToByteArray(this string input)
        {
            return input.ToBytesForRLPEncoding();
        }

        public static byte[] HexStringToByteArray(this string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static byte[] HexStringToByteArray(this string hex, int length)
        {
            return hex.HexStringToByteArray().AddPadding(length);
        }

        /// <summary>
        /// Checks if the string is a valid hex string, containing only hex characters after a
        /// possible '0x'.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static bool IsHexString(this string test)
        {
            return Regex.IsMatch(test, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
        }

        /// <summary>
        /// A valid VeChain address should be 42 characters long and start with 0x and contain only
        /// hex characters after the 0x.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsValidAddress(string address)
        {
            return address.Length == 42 && address.StartsWith("0x") && address.IsHexString();
        }

        public static byte[] ToBytesPadded(BigInteger value, int length)
        {
            byte[] result = new byte[length];
            byte[] bytes = value.BigIntegerToBytes();
            int bytesLength;
            byte srcOffset;
            if (bytes[0] == 0)
            {
                bytesLength = bytes.Length - 1;
                srcOffset = 1;
            }
            else
            {
                bytesLength = bytes.Length;
                srcOffset = 0;
            }

            if (bytesLength > length)
                throw new ArgumentOutOfRangeException(nameof(length), $"Input is too large to put in byte array of size; {bytesLength}>{length}");

            int destOffset = length - bytesLength;
            Unsafe.CopyBlock(ref result[destOffset], ref bytes[srcOffset], (uint)bytesLength);
            //Array.Copy(bytes, srcOffset, result, destOffset, bytesLength);
            return result;
        }

        public static BigInteger BytesToBigInt(byte[] bytes)
        {
            return new BigInteger(1, bytes);
        }

        public static byte[] AddPadding(this byte[] array, int length)
        {
            byte[] result = new byte[length];

            if (array.Length > length)
                throw new ArgumentOutOfRangeException(nameof(length), $"Input is too large to put in byte array of size; {array.Length}>{length}");

            Unsafe.CopyBlock(ref result[length - array.Length], ref array[0], (uint)array.Length);
            //Array.Copy(array, 0, result, length - array.Length, array.Length);
            return result;
        }
    }
}