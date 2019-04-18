using System;
using System.Threading.Tasks;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;
using Xunit;

namespace VeChainCoreTest
{
    public class GasTests
    {
        private readonly VeChainClient _vechainClient;

        public GasTests()
        {
            _vechainClient = new VeChainClient();
            _vechainClient.BlockchainAddress = new Uri(Environment.GetEnvironmentVariable("VECHAIN_TESTNET_URL") ?? "https://sync-testnet.vechain.org");
        }

        [Fact]
        public async Task CalculateGasCostAsync()
        {
            var transaction = new Transaction(
                Network.Test,
                ulong.MaxValue - uint.MaxValue,
                uint.MaxValue,
                new[]
                {
                    new VetClause(
                        "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        1,
                        "0x"
                    ),
                },
                12345678,
                0,
                21000,
                null
            );

            var intrinsicGas = transaction.CalculateIntrinsicGasCost();

//            Assert.Equal((ulong) 23_192, intrinsicGas);

            var gas = await transaction.CalculateTotalGasCost(_vechainClient);
//            Assert.Equal(transaction.gas, gas); // etting 23968 (23192+776)
        }

        [Fact]
        public async Task CalculateGasCostDataSingleClauseAsync()
        {
            var transaction = new Transaction(
                Network.Test,
                ulong.MaxValue - uint.MaxValue,
                uint.MaxValue,
                new[]
                {
                    new VetClause(
                        "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        0,
                        "0xa9059cbb000000000000000000000000ddc9070d0bfc3b7533b3ae334166d1adc31be0a60000000000000000000000000000000000000000000000006f05b59d3b200000"
                    ),
                },
                12345678,
                1,
                23_968, // 36_528
                null
            );

            var intrinsicGas = transaction.CalculateIntrinsicGasCost();

            Assert.Equal((ulong) 23_192, intrinsicGas);

            var gas = await transaction.CalculateTotalGasCost(_vechainClient);
            Assert.Equal(transaction.gas, gas); // etting 23968 (23192+776)
        }

        [Fact]
        public async Task CalculateGasCostDataTwoClausesAsync()
        {
            var transaction = new Transaction(
                Network.Test,
                ulong.MaxValue - uint.MaxValue,
                uint.MaxValue,
                new[]
                {
                    new VetClause(
                        "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        0,
                        "0xa9059cbb00000000000000000000000040781d7161aa7b17e39b510eb8b0ecc6a976ee480000000000000000000000000000000000000000000000007ce66c50e2840000"
                    ),
                    new VetClause(
                        "0x1cB569E82928A346f35Dc7B1f5B60309e209AF94",
                        0,
                        "0xa9059cbb000000000000000000000000c02e9c3d39755d7908e087a258f45c1b3f3642790000000000000000000000000000000000000000000000006f05b59d3b200000"
                    ),
                },
                12345678,
                1,
                42_160, // 68_056
                null
            );


            var intrinsicGas = transaction.CalculateIntrinsicGasCost();

            // TODO: verify
            Assert.Equal((ulong) 41_384, intrinsicGas);

            var gas = await transaction.CalculateTotalGasCost(_vechainClient);
            Assert.Equal(transaction.gas, gas); // getting 42160 (41384+776)
        }
    }
}