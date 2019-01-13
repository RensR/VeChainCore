using System.Threading.Tasks;

using Org.BouncyCastle.Math;
using VeChainCore.Client;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils.Rlp;
using Xunit;

namespace VeChainCoreTest
{
    public class TransactionTest
    {
        private readonly VeChainClient _vechainClient;

        public TransactionTest()
        {
            _vechainClient = new VeChainClient();
            _vechainClient.SetBlockchainAddress("http://localhost:8669");
        }


        [Fact]
        public async Task CreateTransaction()
        {
            var chainTag = await _vechainClient.GetChainTag();
            var blockref = "0x001a7d4448f0948b"; // await _vechainClient.GetLatestBlockRef();


            var trans = RawTransaction.CreateUnsigned(chainTag, blockref, new[]
            {
                new RawClause("0xd221f5d437a49813cdcecf99856f0a0bde529c6a", "1", "", false)

            }, "12345678", 720, 0, 21000, "");

            var rlpTransaction = new RlpTransaction(trans).AsRlpValues();

            var rawWithoutMeta =
                "2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478";



            
           

            var asHexString = RlpEncoder.Encode(rlpTransaction);

            var rawTransaction = asHexString.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);

            // 0x44C3e1Ce754129Eb74522E3CA5695B7Cfa6d2B19
            var privateKey = new BigInteger("0x58beea15dd1835da3a866169d0c0d553b8a740a99cc7859dd8456509790b7e36".HexStringToByteArray());
            var publicKey = ECDSASign.PublicKeyFromPrivate(privateKey);


            var customKey = new ECKeyPair(privateKey, publicKey);

            trans.Sign(customKey).CalculateTxId(new Address(customKey.GetHexAddress())).Transfer();


            var hash = Hash.HashBlake2B(asHexString);
            var hashString = hash.ByteArrayToString();

            //Assert.Equal(rawWithoutMeta, hash.ToString());
        }
    }
}
