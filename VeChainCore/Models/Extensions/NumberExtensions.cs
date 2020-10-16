using Nethereum.Hex.HexConvertors.Extensions;
using VeChainCore.Models.Core;

namespace VeChainCore.Models.Extensions
{
    public static class NumberExtensions
    {
        public static byte[] AmountToBytes(this Amount amount)
        {
            return amount.AsBigInt.ToByteArrayUnsigned();
        }

        public static string AmountToHex(this Amount amount)
        {
            return amount.AmountToBytes().ToHex();
        }

        public static string ToHex(this ulong number)
        {
            return number.ToBigEndianBytes().ToHex();
        }

        public static string ToHex(this decimal number)
        {
            return number.ToBigInteger().ToByteArrayUnsigned().ToHex();
        }
    }
}