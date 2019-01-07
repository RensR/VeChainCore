using System.Numerics;
using VeChainCore.Utils;
using Xunit;

namespace VeChainCoreTest
{
    public class LogicTests
    {
        [Fact]
        public void ConvertHexToBigInt()
        {
            Assert.Equal((ulong) 17, Hex.HexToBigInt("11"));
            Assert.Equal((ulong) 33, Hex.HexToBigInt("0x21"));
            Assert.Equal((ulong) 1000000000000000000, Hex.HexToBigInt("de0b6b3a7640000"));
            Assert.Equal((ulong) 1072057594037927936, Hex.HexToBigInt("ee0b6b3a7640000"));
            Assert.Equal(BigInteger.Parse("37499989800010010000100000010"), 
                Hex.HexToBigInt("0x792b43b877bed2e147c9810a"));
        }

        [Fact]
        public void ConvertHexToDecimal()
        {
            Assert.Equal(17m, Hex.HexToDecimal("11"));
            Assert.Equal(33m, Hex.HexToDecimal("0x21"));
            Assert.Equal(1000000000000000000m, Hex.HexToDecimal("de0b6b3a7640000"));
            Assert.Equal(37499989800010010000100000010m,
                Hex.HexToDecimal("0x792b43b877bed2e147c9810a"));
        }

        [Fact]
        public void ConvertHexToHumanReadableDecimal()
        {
            Assert.Equal(37499989800.010010000100000010m,
                Hex.HexToHumanReadableDecimal("0x792b43b877bed2e147c9810a"));
            Assert.Equal(1.072057594037927936m, Hex.HexToHumanReadableDecimal("ee0b6b3a7640000"));
            Assert.Equal(1, Hex.HexToHumanReadableDecimal("de0b6b3a7640000"));
        }
    }
}
