using VeChainCore.Logic.Cryptography;
using VeChainCore.Logic;
using VeChainCore.Models;
using VeChainCore.Models.Transaction;
using Xunit;
using Transaction = VeChainCore.Models.Transaction;
namespace VeChainCoreTest
{
    public class RLPTest
    {

        [Fact]
        public void RLPHexParser()
        {
            var realTransaction = new Transaction.Transaction
            {
                chainTag = 1,
                blockRef = "0x00000000aabbccdd",
                expiration = 32,
                clauses = new[]{ new Clause{
                        to= "0x7567d83b7b8d80addcb281a71d54fc7b3364ffed",
                        value= "10000",
                        data= "0x000000606060"}
                    , new Clause
                    {
                        to= "0x7567d83b7b8d80addcb281a71d54fc7b3364ffed",
                        value= "20000",
                        data= "0x000000606060"
                    }},
                gasPriceCoef = 128,
                gas = 21000,
                dependsOn = null,
                nonce = "12345678"
            };

            var rlpTransaction = new RlpTransaction(realTransaction);

            var vetEncoded = RlpEncoder.Encode(rlpTransaction.AsRLPValues());
           
            var out1 = Hex.ByteArrayToString(vetEncoded);
            var vethash = Hash.HashBlake2B(vetEncoded);

            // Should be 2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478
            var vethashReadable = Hex.ByteArrayToString(vethash);
        }

        [Fact]
        public void DecoderTest()
        {
            string hexRaw = "0xf83d81c7860881eec535498202d0e1e094000000002beadb038203be21ed5ce7c9b1bff60289056bc75e2d63100000808082520880884773cc184328eb3ec0";
            var rlpList = RlpDecoder.decode(hexRaw.StringToByteArray());
            byte[] encoded = RlpEncoder.Encode(rlpList);
            string hexEncoded = encoded.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);
            Assert.Equal(hexRaw, hexEncoded);

        }
    }
}
