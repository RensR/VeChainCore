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
            _vechainClient.SetBlockchainAddress("https://sync-testnet.vechain.org");
        }


        [Fact]
        public async Task CreateTransaction()
        {
            var chainTag = await _vechainClient.GetChainTag();
            var blockref = "0x001a7d4448f0948b"; // await _vechainClient.GetLatestBlockRef();


            var trans = RawTransaction.CreateUnsigned(chainTag, blockref, new[]
            {
                new RawClause("0xd3ae78222beadb038203be21ed5ce7c9b1bff602", "1", "", false)

            }, "12345678", 720, 0, 21000, "");

            var rlpTransaction = new RlpTransaction(trans).AsRlpValues();


            var asHexString = RlpEncoder.Encode(rlpTransaction);

            var rawTransaction = asHexString.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);

            // 0x44C3e1Ce754129Eb74522E3CA5695B7Cfa6d2B19
            var privateKey = new BigInteger("0xdce1443bd2ef0c2631adc1c67e5c93f13dc23a41c18b536effbbdcbcdb96fb65".HexStringToByteArray());
            var publicKey = ECDSASign.PublicKeyFromPrivate(privateKey);


            var customKey = new ECKeyPair(privateKey, publicKey);

            var result = trans.Sign(customKey).CalculateTxId(new Address(customKey.GetHexAddress())).Transfer(_vechainClient);



        }
    }
}
