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
            _vechainClient = new VeChainClient
            {
                ServerUri = new Uri(Environment.GetEnvironmentVariable("VECHAIN_TESTNET_URL") ?? "https://sync-testnet.vechain.org")
            };
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
                        "0x5034Aa590125b64023a0262112b98d72e3C8E40e",
                        1,
                        ""
                    ),
                },
                12345678,
                0,
                21000,
                null
            );

            var intrinsicGas = transaction.CalculateIntrinsicGasCost();

            var gas = await transaction.CalculateTotalGasCost(_vechainClient);

            Assert.Equal(transaction.gas, gas);
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
                    new ArbitraryClause(
                        "0x6C593ab3e58601f1acC737B28883aD459Bb4514d",
                        0,
                        "0xa9059cbb" +
                        "000000000000000000000000ddc9070d0bfc3b7533b3ae334166d1adc31be0a6" +
                        "0000000000000000000000000000000000000000000000006f05b59d3b200000"
                    ),
                },
                12345678,
                1,
                23_192, // 36_528 ?
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
                    new ArbitraryClause(
                        "0x6C593ab3e58601f1acC737B28883aD459Bb4514d",
                        0,
                        "0xa9059cbb" +
                        "00000000000000000000000040781d7161aa7b17e39b510eb8b0ecc6a976ee48" +
                        "0000000000000000000000000000000000000000000000007ce66c50e2840000"
                    ),
                    new ArbitraryClause(
                        "0x6C593ab3e58601f1acC737B28883aD459Bb4514d",
                        0,
                        "0xa9059cbb" +
                        "000000000000000000000000c02e9c3d39755d7908e087a258f45c1b3f364279" +
                        "0000000000000000000000000000000000000000000000006f05b59d3b200000"
                    ),
                },
                12345678,
                1,
                41_384, // 68_056 ?
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