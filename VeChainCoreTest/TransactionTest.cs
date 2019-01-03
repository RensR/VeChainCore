using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VeChainCore.Client;
using VeChainCore.Models;
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
            var bestBlock = await _vechainClient.GetBlock("best");

            var transaction = Transaction.CreateUnsigned(chainTag, bestBlock.id, new []{new Clause()});
            transaction.nonce = "0x02";

            var rawTransaction = new RawTransaction(transaction, new byte[0]);

            var encoded = rawTransaction.Encode();
        }
    }
}
