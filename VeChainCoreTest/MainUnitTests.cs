using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VeChainCore.Client;
using VeChainCore.Logic;
using VeChainCore.Models;
using VeChainCore.Models.Extensions;
using VeChainCore.Models.Transaction;
using Xunit;

namespace VeChainCoreTest
{
    public class BlockTest
    {
        private readonly VeChainClient _vechainClient;

        public BlockTest()
        {
            _vechainClient = new VeChainClient();
            _vechainClient.SetBlockchainAddress("http://localhost:8669");
        }


        [Fact]
        public async Task GenesisBlockIdCheckAsync()
        {
            var testnet = await _vechainClient.GetChainTag() == 39;
            var genesis = testnet ? 
                new Block // Test
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
                    transactions = new Transaction[0]
                }
                :
                new Block // Main
                {
                    number = 0,
                    id = "0x00000000851caf3cfdb6e899cf5958bfb1ac3413d346d43539627e6be7ec1b4a",
                    size = 170,
                    parentID = "0xffffffff53616c757465202620526573706563742c20457468657265756d2100",
                    timestamp = 1530316800,
                    gasLimit = 10000000,
                    beneficiary = "0x0000000000000000000000000000000000000000",
                    gasUsed = 0,
                    totalScore = 0,
                    txsRoot = "0x45b0cfc220ceec5b7c1c62c4d4193d38e4eba48e8815729ce75f9c0ab0e4c1c0",
                    stateRoot = "0x09bfdf9e24dd5cd5b63f3c1b5d58b97ff02ca0490214a021ed7d99b93867839c",
                    receiptsRoot = "0x45b0cfc220ceec5b7c1c62c4d4193d38e4eba48e8815729ce75f9c0ab0e4c1c0",
                    signer = "0x0000000000000000000000000000000000000000",
                    isTrunk = true,
                    transactions = new Transaction[0]
                };

            var block = await _vechainClient.GetBlock("0");

            Assert.Equal(genesis, block);
        }

        [Fact]
        public async Task GetAccountBalance()
        {
            // Assert that this address has no contract at the current block
            var account = await _vechainClient.GetAccount("0xa9eb0d2bf88d7a190728879865ea231c3a15d54b", "1591234");

            Assert.True(!account.hasCode);
            Assert.Equal(BigInteger.Parse("21087000000000000000000"), Hex.HexToBigInt(account.balance));

            // Assert that the address had no engery nor contract at genesis
            var genesisAccount = await _vechainClient.GetAccount("0xa9eb0d2bf88d7a190728879865ea231c3a15d54b", "0");
            Assert.False(genesisAccount.hasCode);
            Assert.Equal(0, Hex.HexToBigInt(genesisAccount.energy));
        }

        [Fact]
        public async Task GetTransaction()
        {
            var transaction = await _vechainClient.GetTransaction("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

            Assert.Equal(749, transaction.clauses.Length);
            Assert.Equal("0x00183e68e864ee05", transaction.blockRef);
        }

        [Fact]
        public async Task GetReceipt()
        {
            var receipt = await _vechainClient.GetReciept("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

            Assert.Equal(749, receipt.outputs.Length);
            Assert.Equal("0x3d0296f141deca31be8", receipt.paid);
            Assert.Equal((uint)11989000, receipt.gasUsed);

            var totalSent = receipt.outputs.Sum(output => output.transfers.Sum(transfer => Hex.HexToHumanReadableDecimal(transfer.amount)));
            Assert.Equal(738, totalSent);
        }

        [Fact]
        public async Task TestnetFaucet()
        {
            Assert.Null(await _vechainClient.TestnetFaucet("0x"));
        }

        [Fact]
        public async void CalculateGasCost()
        {
            var transaction = await _vechainClient.GetTransaction("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

            var gas = transaction.CalculateGasCost();

            Assert.Equal(transaction.gas, gas);
        }

        [Fact]
        public async Task GetChainTag()
        {
            var tag = await _vechainClient.GetChainTag();

            // 39 == Testnet
            // 74 == Mainnet
            Assert.True(tag == 39 || tag == 74);
        }
    }
}
