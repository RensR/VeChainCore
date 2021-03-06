using System.Globalization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Org.BouncyCastle.Math;
using Utf8Json;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils.Cryptography;
using Xunit;

namespace VeChainCoreTest
{
    public class EncodingTests
    {
        private const decimal EighteenDecimalPlaces = 1000000000000000000m;
        private static readonly decimal DecimalMaxValueDown18 = decimal.Round(decimal.MaxValue / EighteenDecimalPlaces);

        [Fact]
        public void BigIntToMaxDec()
        {
            var expected = decimal.MaxValue;
            var actual = new BigInteger(decimal.MaxValue.ToString(CultureInfo.InvariantCulture)).ToDecimal();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxDecToBigInt()
        {
            var actual = decimal.MaxValue.ToBigInteger();
            var expected = new BigInteger(decimal.MaxValue.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BigIntToMinDec()
        {
            var expected = decimal.MinValue;
            var actual = new BigInteger(decimal.MinValue.ToString(CultureInfo.InvariantCulture)).ToDecimal();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinDecToBigInt()
        {
            var actual = decimal.MinValue.ToBigInteger();
            var expected = new BigInteger(decimal.MinValue.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BigIntToMaxDecDown18()
        {
            var expected = DecimalMaxValueDown18;
            var actual = new BigInteger(DecimalMaxValueDown18.ToString(CultureInfo.InvariantCulture)).ToDecimal();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxDecDown18ToBigInt()
        {
            var actual = DecimalMaxValueDown18.ToBigInteger();
            var expected = new BigInteger(DecimalMaxValueDown18.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RlpEncode10000m()
        {
            var expected = "0x822710".HexToByteArray();
            var actual = RLP.EncodeElement(10000m.ToBigInteger().ToByteArrayUnsigned().TrimLeading());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RlpEncode20000m()
        {
            var expected = "0x824e20".HexToByteArray();
            var actual = RLP.EncodeElement(20000m.ToBigInteger().ToByteArrayUnsigned().TrimLeading());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RlpEncodeTransaction()
        {
            // based on https://github.com/vechain/thor/blob/d9f618b4974733e04949f7b9424001f5bd572baa/tx/transaction_test.go#L20
            string to = "0x7567d83b7b8d80addcb281a71d54fc7b3364ffed";
            var txn = new Transaction(
                (Network) 1,
                0x00000000aabbccdd,
                32,
                new[]
                {
                    new VetClause(to, 10000m, "0x000000606060"),
                    new VetClause(to, 20000m, "0x000000606060")
                },
                12345678,
                128,
                21000,
                null
            );

            var rlpSigData = txn.RlpDataForSignature;
            var expectedSigRlpHex = "0xf8540184aabbccdd20f840df947567d83b7b8d80addcb281a71d54fc7b3364ffed82271086000000606060df947567d83b7b8d80addcb281a71d54fc7b3364ffed824e208600000060606081808252088083bc614ec0";
            var actualSigRlpHex = rlpSigData.ToHex(true);
            Assert.Equal(expectedSigRlpHex, actualSigRlpHex);

            var expectedRlpHex = "0xf8550184aabbccdd20f840df947567d83b7b8d80addcb281a71d54fc7b3364ffed82271086000000606060df947567d83b7b8d80addcb281a71d54fc7b3364ffed824e208600000060606081808252088083bc614ec080";
            var actualRlpHex = txn.RLPData.ToHex(true);
            Assert.Equal(expectedRlpHex, actualRlpHex);

            var signingHashHex = Hash.Blake2B(rlpSigData).ToHex(true);
            Assert.Equal("0x2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478", signingHashHex);

            var ec = ECKeyPair.Create("0x7582be841ca040aa940fff6c05773129e135623e41acce3e0b8ba520dc1ae26a");

            txn.Sign(ec);

            var expectedSignatureHex = "0xf76f3c91a834165872aa9464fc55b03a13f46ea8d3b858e528fcceaf371ad6884193c3f313ff8effbb57fe4d1adc13dceb933bedbf9dbb528d2936203d5511df00";
            Assert.Equal(expectedSignatureHex, txn.signature);

            // signer: 0xd989829d88b0ed1b06edf5c50174ecfa64f14a64
            // id: 0xda90eaea52980bc4bb8d40cb2ff84d78433b3b4a6e7d50b75736c5e3e77b71ec

            var expectedSignedRlpHex =
                "0xf8970184aabbccdd20f840df947567d83b7b8d80addcb281a71d54fc7b3364ffed82271086000000606060df947567d83b7b8d80addcb281a71d54fc7b3364ffed824e208600000060606081808252088083bc614ec0b841f76f3c91a834165872aa9464fc55b03a13f46ea8d3b858e528fcceaf371ad6884193c3f313ff8effbb57fe4d1adc13dceb933bedbf9dbb528d2936203d5511df00";
            var actualSignedRlpHex = txn.RLPData.ToHex(true);

            Assert.Equal(expectedSignedRlpHex, actualSignedRlpHex);

            var jsonBytes = JsonSerializer.Serialize(txn, VeChainClient.JsonFormatterResolver);

            var json = JsonSerializer.PrettyPrint(jsonBytes);

            Assert.Equal(@"{
  ""chainTag"": 1,
  ""blockRef"": ""0x00000000aabbccdd"",
  ""expiration"": 32,
  ""clauses"": [
    {
      ""to"": ""0x7567d83b7b8d80addcb281a71d54fc7b3364ffed"",
      ""value"": ""0x2710"",
      ""data"": ""0x000000606060""
    },
    {
      ""to"": ""0x7567d83b7b8d80addcb281a71d54fc7b3364ffed"",
      ""value"": ""0x4e20"",
      ""data"": ""0x000000606060""
    }
  ],
  ""gasPriceCoef"": 128,
  ""gas"": 21000,
  ""nonce"": ""0xbc614e"",
  ""signature"": ""0xf76f3c91a834165872aa9464fc55b03a13f46ea8d3b858e528fcceaf371ad6884193c3f313ff8effbb57fe4d1adc13dceb933bedbf9dbb528d2936203d5511df00""
}", json);
        }

        [Fact]
        public void TransactionJson()
        {
            var t = new Transaction(
                Network.Test,
                0x0fffffff,
                720,
                new Clause[]
                {
                },
                0x0fffffff,
                byte.MaxValue,
                21000,
                ""
            );

            var json = JsonSerializer.ToJsonString(t, VeChainClient.JsonFormatterResolver);

            Assert.Equal("{\"chainTag\":39,\"blockRef\":\"0x000000000fffffff\",\"expiration\":720,\"clauses\":[],\"gasPriceCoef\":255,\"gas\":21000,\"nonce\":\"0xfffffff\"}", json);

            //var pretty = JsonSerializer.PrettyPrint(json);
        }

        [Fact]
        public void TransactionWithClauseJson()
        {
            var t = new Transaction(
                Network.Test,
                0x0fffffff,
                720,
                new Clause[]
                {
                    new VetClause("0x7567d83b7b8d80addcb281a71d54fc7b3364ffed", 100000m, "0x000000606060")
                },
                0x0fffffff,
                byte.MaxValue,
                21000,
                ""
            );

            var json = JsonSerializer.ToJsonString(t, VeChainClient.JsonFormatterResolver);

            Assert.Equal(
                "{\"chainTag\":39,\"blockRef\":\"0x000000000fffffff\",\"expiration\":720,\"clauses\":[{\"to\":\"0x7567d83b7b8d80addcb281a71d54fc7b3364ffed\",\"value\":\"0x0186a0\",\"data\":\"0x000000606060\"}],\"gasPriceCoef\":255,\"gas\":21000,\"nonce\":\"0xfffffff\"}",
                json);
            //Assert.Equal("{\"chainTag\":39,\"blockRef\":\"0x000000000fffffff\",\"expiration\":720,\"clauses\":[{\"to\":\"0x7567d83b7b8d80addcb281a71d54fc7b3364ffed\",\"value\":\"0x186a0\",\"data\":\"0x000000606060\"}],\"gasPriceCoef\":255,\"gas\":21000,\"dependsOn\":\"\",\"nonce\":\"0xfffffff\"}", json);

            //var pretty = JsonSerializer.PrettyPrint(json);
        }

        [Fact]
        public void TransactionWithClauseJson2()
        {
            var t = new Transaction(
                Network.Test,
                0x0fffffff,
                720,
                new Clause[]
                {
                    new VetClause("0x7567d83b7b8d80addcb281a71d54fc7b3364ffed", 56, "0x000000606060")
                },
                0x0fffffff,
                byte.MaxValue,
                21000,
                ""
            );

            var json = JsonSerializer.ToJsonString(t, VeChainClient.JsonFormatterResolver);

            Assert.Equal(
                "{\"chainTag\":39,\"blockRef\":\"0x000000000fffffff\",\"expiration\":720,\"clauses\":[{\"to\":\"0x7567d83b7b8d80addcb281a71d54fc7b3364ffed\",\"value\":\"0x38\",\"data\":\"0x000000606060\"}],\"gasPriceCoef\":255,\"gas\":21000,\"nonce\":\"0xfffffff\"}",
                json);
            //Assert.Equal("{\"chainTag\":39,\"blockRef\":\"0x000000000fffffff\",\"expiration\":720,\"clauses\":[{\"to\":\"0x7567d83b7b8d80addcb281a71d54fc7b3364ffed\",\"value\":\"0x186a0\",\"data\":\"0x000000606060\"}],\"gasPriceCoef\":255,\"gas\":21000,\"dependsOn\":\"\",\"nonce\":\"0xfffffff\"}", json);

            //var pretty = JsonSerializer.PrettyPrint(json);
        }
    }
}