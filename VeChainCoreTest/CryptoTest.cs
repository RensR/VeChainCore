using VeChainCore.Logic;
using VeChainCore.Logic.Cryptography;
using Xunit;

namespace VeChainCoreTest
{
    public class CryptoTest
    {
        [Fact]
        public void TestBlake2B()
        {
            var helloword = "Hello world";
            byte[] helloBytes = helloword.StringToByteArray();
            byte[] blake2b = Hash.HashBlake2B(helloBytes);
            string hexString = Hex.ByteArrayToString(blake2b, StringType.Hex, Prefix.ZeroLowerX);

            Assert.Equal("0xa21cf4b3604cf4b2bc53e6f88f6a4d75ef5ff4ab415f3e99aea6b61c8249c4d0", hexString);
        }
    }
}
