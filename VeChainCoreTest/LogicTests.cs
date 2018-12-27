using System.Numerics;
using VeChainCore.Logic;
using Xunit;

namespace VeChainCoreTest
{
    public class LogicTests
    {
        [Fact]
        public void ConvertHexToBigInt()
        {
            Assert.Equal((ulong) 17, HexConverter.HexToBigInt("11"));
            Assert.Equal((ulong) 33, HexConverter.HexToBigInt("0x21"));
            Assert.Equal((ulong) 1000000000000000000, HexConverter.HexToBigInt("de0b6b3a7640000"));
            Assert.Equal((ulong) 1072057594037927936, HexConverter.HexToBigInt("ee0b6b3a7640000"));
            Assert.Equal(BigInteger.Parse("37499989800010010000100000010"), 
                HexConverter.HexToBigInt("0x792b43b877bed2e147c9810a"));
        }

        [Fact]
        public void ConvertHexToDecimal()
        {
            Assert.Equal(17m, HexConverter.HexToDecimal("11"));
            Assert.Equal(33m, HexConverter.HexToDecimal("0x21"));
            Assert.Equal(1000000000000000000m, HexConverter.HexToDecimal("de0b6b3a7640000"));
            Assert.Equal(37499989800010010000100000010m,
                HexConverter.HexToDecimal("0x792b43b877bed2e147c9810a"));
        }

        [Fact]
        public void ConvertHexToHumanReadableDecimal()
        {
            Assert.Equal(37499989800.010010000100000010m,
                HexConverter.HexToHumanReadableDecimal("0x792b43b877bed2e147c9810a"));
            Assert.Equal(1.072057594037927936m, HexConverter.HexToHumanReadableDecimal("ee0b6b3a7640000"));
            Assert.Equal(1, HexConverter.HexToHumanReadableDecimal("de0b6b3a7640000"));
        }
    }
}
