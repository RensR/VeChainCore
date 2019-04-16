using System;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;
using VeChainCore.Utils;
using Xunit;

namespace VeChainCoreTest
{
    public class TestNetIntegrationTests
    {
        private readonly VeChainClient _vechainClient;

        public TestNetIntegrationTests()
        {
            _vechainClient = new VeChainClient();
            _vechainClient.BlockchainAddress = new Uri("https://sync-testnet.vechain.org");
        }

        [Fact]
        public async Task GetChainTag()
        {
            var tag = await _vechainClient.GetChainTag();
            Assert.True(tag == Network.Test);
        }

        [Fact]
        public async Task GetTransaction()
        {
            var transaction = await _vechainClient.GetTransaction("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

            Assert.Equal(749, transaction.clauses.Length);
            // TODO
            //Assert.Equal("0x00183e68e864ee05", transaction.blockRef);
        }

        [Fact]
        public async Task GetReceipt()
        {
            var receipt = await _vechainClient.GetReceipt("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

            Assert.Equal(749, receipt.outputs.Length);
            Assert.Equal("0x3d0296f141deca31be8", receipt.paid);
            Assert.Equal((uint) 11989000, receipt.gasUsed);
        }


        [Fact]
        public async Task GetAccountBalance()
        {
            // Assert that this address has no contract at the current block
            var account = await _vechainClient.GetAccount("0xa9eb0d2bf88d7a190728879865ea231c3a15d54b", "1591234");

            Assert.True(!account.hasCode);
            Assert.Equal(new BigInteger("21087000000000000000000"), account.balance.HexToByteArray().ToBigInteger());

            // Assert that the address had no energy nor contract at genesis
            var genesisAccount = await _vechainClient.GetAccount("0xa9eb0d2bf88d7a190728879865ea231c3a15d54b", "0");
            Assert.False(genesisAccount.hasCode);
            Assert.Equal(BigInteger.Zero, genesisAccount.energy.HexToByteArray().ToBigInteger());
        }


        [Fact]
        public async Task GenesisBlockIdCheckAsync()
        {
            var genesis = new Block // Test
            {
                number = 0,
                id = "0x000000000b2bce3c70bc649a02749e8687721b09ed2e15997f466536b20bb127",
                size = 170,
                parentID = "0xffffffff00000000000000000000000000000000000000000000000000000000",
                timestamp = 1530014400,
                gasLimit = 10000000,
                beneficiary = "0x0000000000000000000000000000000000000000",
                gasUsed = 0,
                totalScore = 0,
                txsRoot = "0x45b0cfc220ceec5b7c1c62c4d4193d38e4eba48e8815729ce75f9c0ab0e4c1c0",
                stateRoot = "0x4ec3af0acbad1ae467ad569337d2fe8576fe303928d35b8cdd91de47e9ac84bb",
                receiptsRoot = "0x45b0cfc220ceec5b7c1c62c4d4193d38e4eba48e8815729ce75f9c0ab0e4c1c0",
                signer = "0x0000000000000000000000000000000000000000",
                isTrunk = true,
                transactions = new string[0]
            };

            var block = await _vechainClient.GetBlock("0");

            Assert.Equal(genesis, block);
        }
    }
}