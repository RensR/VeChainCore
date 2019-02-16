using VeChainCore.Utils;
using Xunit;

namespace VeChainCoreTest
{
    public class UtilTest
    {

        [Theory]
        [InlineData(new byte[] { 0x43 }, 5, new byte[] { 0x0, 0x0, 0x0, 0x0, 0x43 })]
        public void ArrayPaddingTest(byte[] bytes, int length, byte[] expected)
        {
            var withPadding = bytes.AddPadding(length);

            Assert.Equal(length, withPadding.Length);
            Assert.Equal(expected, withPadding);
        }
    }
}
