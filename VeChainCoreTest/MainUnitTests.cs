using System.Threading.Tasks;
using Org.BouncyCastle.Math;
using VeChainCore.Client;
using VeChainCore.Utils;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Extensions;
using Xunit;
using static VeChainCore.Client.VeChainClient;

namespace VeChainCoreTest
{
    public class BlockTest
    {
        private readonly VeChainClient _vechainClient;

        public BlockTest()
        {
            _vechainClient = new VeChainClient();
            _vechainClient.SetBlockchainAddress("http://192.168.178.155:8669");
        }

        [Fact]
        public async Task GenesisBlockIdCheckAsync()
        {
            var chainTag = await _vechainClient.GetChainTag();
            var genesis = chainTag == (byte) Network.Test ?
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
                    transactions = new string[0]
                }
                : chainTag == (byte)Network.Main ?
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
                    transactions = new string[0]
                } :
                new Block // Dev
                {
                    number = 0,
                    id = "0x00000000973ceb7f343a58b08f0693d6701a5fd354ff73d7058af3fba222aea4",
                    size = 170,
                    parentID = "0xffffffff00000000000000000000000000000000000000000000000000000000",
                    timestamp = 1526400000,
                    gasLimit = 10000000,
                    beneficiary = "0x0000000000000000000000000000000000000000",
                    gasUsed = 0,
                    totalScore = 0,
                    txsRoot = "0x45b0cfc220ceec5b7c1c62c4d4193d38e4eba48e8815729ce75f9c0ab0e4c1c0",
                    stateRoot = "0x278b34bdbc5294d0cbbb7f1c49100c821e6fff7abc69a0c398c8f27d00563a8e",
                    receiptsRoot = "0x45b0cfc220ceec5b7c1c62c4d4193d38e4eba48e8815729ce75f9c0ab0e4c1c0",
                    signer = "0x0000000000000000000000000000000000000000",
                    isTrunk = true,
                    transactions = new string[0]
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
            Assert.Equal(new BigInteger("21087000000000000000000"), Hex.HexToBigInt(account.balance));

            // Assert that the address had no engery nor contract at genesis
            var genesisAccount = await _vechainClient.GetAccount("0xa9eb0d2bf88d7a190728879865ea231c3a15d54b", "0");
            Assert.False(genesisAccount.hasCode);
            Assert.Equal(new BigInteger("0"), Hex.HexToBigInt(genesisAccount.energy));
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
            Assert.Equal((uint)11989000, receipt.gasUsed);
        }

        [Fact]
        public async Task TestnetFaucet()
        {
            Assert.Null(await _vechainClient.TestNetFaucet("0x"));
        }

        [Fact]
        public async Task CalculateGasCostDataTwoClausesAsync()
        {
            var transaction = new Transaction
            {
                clauses = new Clause[]
                {
                    new Clause
                    {
                        to = "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        value = "0",
                        data = "0xa9059cbb00000000000000000000000040781d7161aa7b17e39b510eb8b0ecc6a976ee480000000000000000000000000000000000000000000000007ce66c50e2840000"
                    },
                    new Clause
                    {
                        to = "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        value = "0",
                        data = "0xa9059cbb000000000000000000000000c02e9c3d39755d7908e087a258f45c1b3f3642790000000000000000000000000000000000000000000000006f05b59d3b200000"
                    }
                },
                gas = 68_056
            };

            var gas = await transaction.CalculateTotalGasCost(_vechainClient);
            var intrinsicGas = transaction.CalculateIntrinsicGasCost();

            Assert.Equal((ulong)23_192, intrinsicGas);

            Assert.Equal(transaction.gas, gas);
        }

        [Fact]
        public async Task CalculateGasCostDataSingleClauseAsync()
        {
            var transaction = new Transaction
            {
                clauses = new Clause[]
                {
                    new Clause
                    {
                        to = "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        value = "0",
                        data = "0xa9059cbb00000000000000000000000031a2f4e3567b29012c3332dfea1f3984487246b30000000000000000000000000000000000000000000000004563918244f40000"
                    }
                },
                gas = 36_528
            };

            var gas = await transaction.CalculateTotalGasCost(_vechainClient);
            var intrinsicGas = transaction.CalculateIntrinsicGasCost();

            Assert.Equal((ulong) 23_192, intrinsicGas);
            
           // Assert.Equal(transaction.gas, gas);
        }

        [Fact]
        public async Task GetChainTag()
        {
            var tag =  await _vechainClient.GetChainTag();
            Assert.True(tag == (byte)Network.Test || tag == (byte)Network.Main || tag == (byte)Network.Dev);
        }
    }
}
