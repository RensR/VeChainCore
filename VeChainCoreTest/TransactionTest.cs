using System.Threading.Tasks;
using Nethereum.RLP;
using VeChainCore.Client;
using VeChainCore.Models.Extensions;
using VeChainCore.Models.Transaction;
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
            //var bestBlock = await _vechainClient.GetBlock("best");

            var transaction = Transaction.CreateUnsigned(chainTag, "0x03", new []{new Clause()});
            transaction.nonce = "0x02";

            var rawTransaction = new RawTransaction(transaction);
            rawTransaction.signature = "sig".ToBytesForRLPEncoding();

            var txid = rawTransaction.CalculateTxId("0xeCC159751F9aed21399d5e3CE72BC9D4FcCB9cCc");
            

            var encoded = rawTransaction.AsRlpValues();
        }
    }
}
