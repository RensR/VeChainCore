using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Math;
using VeChainCore.Client;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;
using Xunit;

namespace VeChainCoreTest
{
    public class TransactionTest
    {
        private readonly VeChainClient _vechainClient;

        public TransactionTest()
        {
            _vechainClient = new VeChainClient();
            _vechainClient.SetBlockchainAddress("https://sync-testnet.vechain.org");
        }


        [Fact]
        public async Task CreateTransaction()
        {
            var chainTag = await _vechainClient.GetChainTag();
            var blockref = "0x001a7d4448f0948b"; // await _vechainClient.GetLatestBlockRef();


            var trans = RlpTransaction.CreateUnsigned(chainTag, blockref, 720, new[]
            {
                new RlpClause("0xd3ae78222beadb038203be21ed5ce7c9b1bff602", "1", "", false)
            }, 12345678, 0, 21000, "");

            var rlpTransaction = trans.RLPData;

            // 0x44C3e1Ce754129Eb74522E3CA5695B7Cfa6d2B19
            var privateKey = new BigInteger("0xdce1443bd2ef0c2631adc1c67e5c93f13dc23a41c18b536effbbdcbcdb96fb65".HexStringToByteArray());
            var publicKey = ECDSASign.PublicKeyFromPrivate(privateKey);


            var customKey = new ECKeyPair(privateKey, publicKey);

            var result = trans.Sign(customKey).CalculateTxId(new Address(customKey.GetHexAddress())).Transfer(_vechainClient);
        }
    }
}