using System.Threading.Tasks;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Core;
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
            var blockref = _vechainClient.GetLatestBlockRef();


            var trans = RawTransaction.CreateUnsigned(chainTag, blockref, new[]
            {
                new RawClause("0xd221f5d437a49813cdcecf99856f0a0bde529c6a", "1", "", false)

            }, "12345678", 720, 0, 21000, "");

            var rlpTransaction = new RlpTransaction(trans).AsRLPValues();

            var rawWithoutMeta =
                "2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478";



            //var txid = rawTransaction.CalculateTxId("0xeCC159751F9aed21399d5e3CE72BC9D4FcCB9cCc");
           

            var asHexString = RlpEncoder.Encode(rlpTransaction);

            var rawTransaction = asHexString.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);

            var hash = Hash.HashBlake2B(asHexString);
            var hashString = hash.ByteArrayToString();

            //Assert.Equal(rawWithoutMeta, hash.ToString());
        }
    }
}
