using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;
using VeChainCore.Models.Core;
using Xunit;
using VeChainCore.Utils.Rlp;
using Nethereum.RLP;
using System.Linq;

namespace VeChainCoreTest
{
    public class RLPTest
    {

        [Fact]
        public void RLPHexParser()
        {
            var realTransaction = new RawTransaction()
            {
                chainTag = 1,
                blockRef = "00000000aabbccdd",
                expiration = 32,
                clauses = new[]{
                    new RawClause("0x7567d83b7b8d80addcb281a71d54fc7b3364ffed", "10000", "0x000000606060", false),
                    new RawClause("0x7567d83b7b8d80addcb281a71d54fc7b3364ffed","20000", "0x000000606060", false)
                },
                gasPriceCoef = 128,
                gas = 21000,
                dependsOn = null,
                nonce = "12345678"
            };

            var rlpTransaction = new RlpTransaction(realTransaction).AsRlpValues();

            var vetEncoded = RlpEncoder.Encode(rlpTransaction);
           
            var out1 = vetEncoded.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);
            var vethash = Hash.HashBlake2B(vetEncoded);

            // Should be 2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478
            var vethashReadable = vethash.ByteArrayToString();

            Assert.Equal("0x2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478", vethashReadable);
        }

        [Fact]
        public void DecoderTest()
        {
            string hexRaw = "0xf83d81c7860881eec535498202d0e1e094000000002beadb038203be21ed5ce7c9b1bff60289056bc75e2d63100000808082520880884773cc184328eb3ec0";
            var rlpList = RlpDecoder.Decode(hexRaw.HexStringToByteArray());

            // The list should only have 1 element
            Assert.Single(rlpList);
            byte[] encodedinner = RlpEncoder.Encode(rlpList[0]);
            string hexEncodedinner = encodedinner.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);
            Assert.Equal(hexRaw, hexEncodedinner);

        }

        [Fact]
        public void DecodeStaticString()
        {
            var input =
                "f902d6819a8702288058b9af928202d0f90273e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f800008001830616988088ff9198c817655decc0b841bd61e198f126adddb169eebf5cd3da25ae3a3f07102e574bcd1368440d1e307c4c47884364e2abc66ef6940c4953758dd1c57f8255025639702104ce83e9a3b501";

            var rlpList = RlpDecoder.Decode(input.HexStringToByteArray());
            var nethereumRLP= RLP.Decode(input.HexStringToByteArray());


            Assert.True(RlpCompare(rlpList, nethereumRLP));
        }

        public bool RlpCompare(IRlpType rlpType, IRLPElement iRLPElement)
        {
            if(rlpType is RlpList vCollection)
            {
                if (iRLPElement is RLPCollection nCollection && vCollection.Count == nCollection.Count)
                {
                    for (var i = 0; i < vCollection.Count; i++)
                    {
                        if (!RlpCompare(vCollection[i], nCollection[i]))
                            return false;
                    }
                }
                else return false;
            }
            else
            {
                var vItem = ((RlpString)rlpType).GetBytes();

                // Is it true that null is the same as an empty array?
                if (iRLPElement.RLPData == null && vItem.Length == 0)
                    return true;

                return vItem.SequenceEqual(iRLPElement.RLPData);
            }
            return true;
        }
    }
}
