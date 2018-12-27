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
            Assert.Equal(BigInteger.Parse("37499989800010010000100000010"), 
                HexConverter.HexToBigInt("0x792b43b877bed2e147c9810a"));
        }
    }
}
