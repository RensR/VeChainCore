using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;
using VeChainCore.Models.Core;
using Xunit;
using Nethereum.RLP;
using System.Linq;
using Nethereum.Hex.HexConvertors.Extensions;

namespace VeChainCoreTest
{
    public class RLPTest
    {
        [Fact]
        public void RLPHexParser()
        {
            // based on https://github.com/vechain/thor/blob/d9f618b4974733e04949f7b9424001f5bd572baa/tx/transaction_test.go#L20
            string to = "0x7567d83b7b8d80addcb281a71d54fc7b3364ffed";
            var rlpTxn = new RlpTransaction
            {
                chainTag = 1,
                blockRef = "00000000aabbccdd",
                expiration = 32,
                clauses = new[]
                {
                    new RlpClause(to, "10000", "0x000000606060", false),
                    new RlpClause(to, "20000", "0x000000606060", false)
                },
                gasPriceCoef = 128,
                gas = 21000,
                dependsOn = null,
                nonce = 12345678
            };

            //var rlpTransaction = new RlpTransaction(realTransaction).AsRlpValues();

            //var vetEncoded = RlpEncoder.Encode(rlpTransaction);

            var rlpData = rlpTxn.RLPData;
            var rlpSignatureData = rlpTxn.RLPSignatureData;

            var expectedRlpHex = "0xf8550184aabbccdd20f840df947567d83b7b8d80addcb281a71d54fc7b3364ffed82271086000000606060df947567d83b7b8d80addcb281a71d54fc7b3364ffed824e208600000060606081808252088083bc614ec080";
            var actualRlpHex = rlpData.ToHex(true);

            var expectedSignatureRlpHex = "0xf8540184aabbccdd20f840df947567d83b7b8d80addcb281a71d54fc7b3364ffed82271086000000606060df947567d83b7b8d80addcb281a71d54fc7b3364ffed824e208600000060606081808252088083bc614ec0";
            var actualSignatureRlpHex = rlpSignatureData.ToHex(true);
            
            Assert.Equal(expectedRlpHex, actualRlpHex);

            var hash = Hash.HashBlake2B(rlpData);
            var hashHex = hash.ToHex(true);
            
            var signingHash = Hash.HashBlake2B(rlpSignatureData);
            var signingHashHex = signingHash.ToHex(true);

            Assert.Equal("0x4bff7001fec40284d93b4c45617dc741b9a88ed59d0f01a376394c7bfea6ed24", hashHex);
            Assert.Equal("0x2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478", signingHashHex);
        }
    }
}