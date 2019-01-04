using System.Globalization;
using System.Numerics;

namespace VeChainCore.Logic
{
    public static class Hex
    {
        public static BigInteger HexToBigInt(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);


            // To counteract the strange sign bit https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.parse?redirectedfrom=MSDN&view=netframework-4.7.2#System_Numerics_BigInteger_Parse_System_String_System_Globalization_NumberStyles_
            hex = "00" + hex;

            return BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier);
        }

        public static decimal HexToDecimal(string hex)
        {
            return (decimal) HexToBigInt(hex);
        }

        public static decimal HexToHumanReadableDecimal(string hex)
        {
            return HexToDecimal(hex) / 1_000_000_000_000_000_000;
        }

        public static decimal ToHumanReadable(decimal dec)
        {
            return dec / 1_000_000_000_000_000_000;
        }
    }
}
