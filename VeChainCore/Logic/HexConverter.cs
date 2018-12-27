using System.Globalization;
using System.Numerics;

namespace VeChainCore.Logic
{
    public static class HexConverter
    {
        public static BigInteger HexToBigInt(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            return BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier);
        }
    }
}
