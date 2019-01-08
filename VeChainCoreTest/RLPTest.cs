using System.Collections.Generic;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;
using VeChainCore.Models.Transaction;
using Xunit;
using Transaction = VeChainCore.Models.Transaction;
using VeChainCore.Utils.Rlp;

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
                blockRef = "00000000aabbccdd",
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
                    }
                },
                gasPriceCoef = 128,
                gas = 21000,
                dependsOn = null,
                nonce = "12345678"
            };

            var rlpTransaction = new RlpTransaction(realTransaction).AsRLPValues();

            var vetEncoded = RlpEncoder.Encode(rlpTransaction);
           
            var out1 = vetEncoded.ByteArrayToString();
            var vethash = Hash.HashBlake2B(vetEncoded);

            // Should be 2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478
            var vethashReadable = vethash.ByteArrayToString();

            Assert.Equal("2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478", vethashReadable);
        }

        [Fact]
        public void DecoderTest()
        {
            string hexRaw = "0xf83d81c7860881eec535498202d0e1e094000000002beadb038203be21ed5ce7c9b1bff60289056bc75e2d63100000808082520880884773cc184328eb3ec0";
            var rlpList = RlpDecoder.decode(hexRaw.HexStringToByteArray());

            // The list should only have 1 element
            Assert.Single(rlpList.GetValues());
            byte[] encodedinner = RlpEncoder.Encode(rlpList.GetValues()[0]);
            string hexEncodedinner = encodedinner.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);
            Assert.Equal(hexRaw, hexEncodedinner);

        }

        [Fact]
        public void DecodeStaticString()
        {
            var input =
                "f902d6819a8702288058b9af928202d0f90273e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f8000080e094d3ef28df6b553ed2fc47259e8134319cb1121a2a89364200111c48f800008001830616988088ff9198c817655decc0b841bd61e198f126adddb169eebf5cd3da25ae3a3f07102e574bcd1368440d1e307c4c47884364e2abc66ef6940c4953758dd1c57f8255025639702104ce83e9a3b501";

            var rlpList = RlpDecoder.decode(input.HexStringToByteArray());
            var other= Nethereum.RLP.RLP.Decode(input.HexStringToByteArray());
        }
    }
}
