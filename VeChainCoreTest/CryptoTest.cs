using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;
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
            string hexString = blake2b.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);

            Assert.Equal("0xa21cf4b3604cf4b2bc53e6f88f6a4d75ef5ff4ab415f3e99aea6b61c8249c4d0", hexString);
        }

        [Fact]
        public void TestKeccak()
        {
            var helloword = "Hello world";
            byte[] helloBytes = helloword.StringToByteArray();
            byte[] Keccac256 = Hash.Keccac256(helloBytes);
            string hexString = Keccac256.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);

            Assert.Equal("0xed6c11b0b5b808960df26f5bfc471d04c1995b0ffd2055925ad1be28d6baadfd", hexString);
        }
    }
}
