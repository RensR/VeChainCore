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
            var blockref = await _vechainClient.GetLatestBlockRef();


            var trans = RawTransaction.CreateUnsigned(chainTag, blockref, new[]
            {
                new RawClause("0xd221f5d437a49813cdcecf99856f0a0bde529c6a", "1", "", false)

            }, "12345678", 720, 0, 21000, "");

            var rlpTransaction = new RlpTransaction(trans).AsRLPValues();

            var rawWithoutMeta =
                "2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478";



            
           

            var asHexString = RlpEncoder.Encode(rlpTransaction);

            var rawTransaction = asHexString.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);


            var privateKey = new BigInteger("0x94b848d4197998738a29e50b047364754b12c98561882d97cddce61f55a18c54".HexStringToByteArray());
            var publicKey = ECDSASign.PublicKeyFromPrivate(privateKey);


            var customKey = new ECKeyPair(privateKey, publicKey);

            trans.Sign(customKey);

            var txid = trans.CalculateTxId(new Address("0xeCC159751F9aed21399d5e3CE72BC9D4FcCB9cCc"));

            var hash = Hash.HashBlake2B(asHexString);
            var hashString = hash.ByteArrayToString();

            //Assert.Equal(rawWithoutMeta, hash.ToString());
        }
    }
}
