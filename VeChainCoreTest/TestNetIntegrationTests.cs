using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Math;
using Utf8Json;
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
            _vechainClient.ServerUri = new Uri(Environment.GetEnvironmentVariable("VECHAIN_TESTNET_URL") ?? "https://sync-testnet.vechain.org");
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
            using (var httpClient = new HttpClient() {BaseAddress = _vechainClient.ServerUri})
            {
                var raw = await httpClient.GetByteArrayAsync("/transactions/0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

                var o = JsonSerializer.Deserialize<dynamic>(raw);

                Assert.Equal(749, o["clauses"].Count);

                var transaction = await _vechainClient.GetTransaction("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

                Assert.Equal(749, transaction.clauses.Count);

                for (var i = 0; i < 749; ++i)
                {
                    var oClause = o["clauses"][i];
                    IClause clause = transaction.clauses[i];

                    Assert.Equal(oClause["to"], clause.to);

                    Assert.Equal(oClause["data"], clause.data);


                    Assert.Equal(
                        ((string) oClause["value"]).HexToByteArray().ToHexCompact(),
                        clause.value.ToBigInteger().ToByteArrayUnsigned().ToHexCompact()
                    );
                }
            }

            // TODO
            //Assert.Equal("0x00183e68e864ee05", transaction.blockRef);
        }

        [Fact]
        public async Task GetTransfersOneAtATime()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000);
            var transfers = _vechainClient.GetTransfers(out var fx, new[]
            {
                new TransferCriteria {recipient = "0xA43751C42125dfB5fFE662F0ad527660885de3AE"},
            }, cts.Token, pageSize: 1);

            var firstTxf = JsonSerializer.Deserialize<Transfer>(@"{
    ""sender"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e"",
    ""recipient"": ""0xa43751c42125dfb5ffe662f0ad527660885de3ae"",
    ""amount"": ""0x1a784379d99db42000000"",
    ""meta"": {
      ""blockID"": ""0x0000855499c6487d85f24d45a68c5a5c457b6b4618cd14ab4073cc8daa2f32ac"",
      ""blockNumber"": 34132,
      ""blockTimestamp"": 1530355720,
      ""txID"": ""0x8c0fa7d8c62b6d3def7fac8cf7d9801cdc809b5225b6ee13ed91c272198244bf"",
      ""txOrigin"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e""
    }
  }");

            bool oneTransfer = false;

            foreach (var transfer in transfers)
            {
                oneTransfer = true;
                cts.Cancel();
                string expected = JsonSerializer.PrettyPrint(JsonSerializer.Serialize(firstTxf, VeChainClient.JsonFormatterResolver));
                string actual = JsonSerializer.PrettyPrint(JsonSerializer.Serialize(transfer, VeChainClient.JsonFormatterResolver));
                Assert.Equal(expected, actual);
                break;
            }

            Assert.True(oneTransfer);

            await fx;


            // heisenbug
            //Assert.Empty(transfers);

            Assert.True(fx.IsCompletedSuccessfully);
        }

        [Fact]
        public void GetTransfersOneAtATimeThrowAwayTask()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000);
            var transfers = _vechainClient.GetTransfers(new[]
            {
                new TransferCriteria {recipient = "0xA43751C42125dfB5fFE662F0ad527660885de3AE"},
            }, cts.Token, pageSize: 1);

            var firstTxf = JsonSerializer.Deserialize<Transfer>(@"{
    ""sender"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e"",
    ""recipient"": ""0xa43751c42125dfb5ffe662f0ad527660885de3ae"",
    ""amount"": ""0x1a784379d99db42000000"",
    ""meta"": {
      ""blockID"": ""0x0000855499c6487d85f24d45a68c5a5c457b6b4618cd14ab4073cc8daa2f32ac"",
      ""blockNumber"": 34132,
      ""blockTimestamp"": 1530355720,
      ""txID"": ""0x8c0fa7d8c62b6d3def7fac8cf7d9801cdc809b5225b6ee13ed91c272198244bf"",
      ""txOrigin"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e""
    }
  }");

            bool oneTransfer = false;

            foreach (var transfer in transfers)
            {
                oneTransfer = true;
                cts.Cancel();
                Assert.Equal(
                    JsonSerializer.PrettyPrint(JsonSerializer.Serialize(firstTxf, VeChainClient.JsonFormatterResolver)),
                    JsonSerializer.PrettyPrint(JsonSerializer.Serialize(transfer, VeChainClient.JsonFormatterResolver))
                );
                break;
            }

            Assert.True(oneTransfer);


            // heisenbug
            //Assert.Empty(transfers);
        }

        [Fact]
        public async Task GetTransfersOneAtATimeForTwo()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(60000);
            var transfers = _vechainClient.GetTransfers(out var fx, new[]
            {
                new TransferCriteria {recipient = "0xA43751C42125dfB5fFE662F0ad527660885de3AE"},
            }, cts.Token, pageSize: 1);

            var firstTxf = JsonSerializer.Deserialize<Transfer>(@"{
    ""sender"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e"",
    ""recipient"": ""0xa43751c42125dfb5ffe662f0ad527660885de3ae"",
    ""amount"": ""0x1a784379d99db42000000"",
    ""meta"": {
      ""blockID"": ""0x0000855499c6487d85f24d45a68c5a5c457b6b4618cd14ab4073cc8daa2f32ac"",
      ""blockNumber"": 34132,
      ""blockTimestamp"": 1530355720,
      ""txID"": ""0x8c0fa7d8c62b6d3def7fac8cf7d9801cdc809b5225b6ee13ed91c272198244bf"",
      ""txOrigin"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e""
    }
  }");
            var secondTxf = JsonSerializer.Deserialize<Transfer>(@"{
    ""sender"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e"",
    ""recipient"": ""0xa43751c42125dfb5ffe662f0ad527660885de3ae"",
    ""amount"": ""0xad78ebc5ac6200000"",
    ""meta"": {
      ""blockID"": ""0x0000855499c6487d85f24d45a68c5a5c457b6b4618cd14ab4073cc8daa2f32ac"",
      ""blockNumber"": 34132,
      ""blockTimestamp"": 1530355720,
      ""txID"": ""0x8c0fa7d8c62b6d3def7fac8cf7d9801cdc809b5225b6ee13ed91c272198244bf"",
      ""txOrigin"": ""0x5034aa590125b64023a0262112b98d72e3c8e40e""
    }
  }");

            bool firstTransfer = false;
            bool secondTransfer = false;

            foreach (var transfer in transfers)
            {
                if (!firstTransfer)
                {
                    firstTransfer = true;
                    Assert.Equal(
                        JsonSerializer.PrettyPrint(JsonSerializer.Serialize(firstTxf, VeChainClient.JsonFormatterResolver)),
                        JsonSerializer.PrettyPrint(JsonSerializer.Serialize(transfer, VeChainClient.JsonFormatterResolver))
                    );
                }
                else
                {
                    secondTransfer = true;
                    cts.Cancel();
                    Assert.Equal(
                        JsonSerializer.PrettyPrint(JsonSerializer.Serialize(secondTxf, VeChainClient.JsonFormatterResolver)),
                        JsonSerializer.PrettyPrint(JsonSerializer.Serialize(transfer, VeChainClient.JsonFormatterResolver))
                    );

                    break;
                }
            }

            Assert.True(firstTransfer);
            Assert.True(secondTransfer);

            await fx;


            // heisenbug
            //Assert.Empty(transfers);

            Assert.True(fx.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task GetReceipt()
        {
            var receipt = await _vechainClient.GetReceipt("0x9b97b53100c7fc27eb17cf38486fdbaa2eb7c8befa41ed0b033ad11fc9c6673e");

            Assert.Equal(749, receipt.outputs.Count);
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
        public async Task GetContractBalance()
        {
            // Assert that this address has a specific balance at the specific block
            decimal balance = await _vechainClient.GetContractBalance("0x9c6e62B3334294D70c8e410941f52D482557955B", "0x71bdd2521C3BA14EBf41aC7f3F4Fb0b7EB1EFbd4", "2590260");
            Assert.Equal(31.885m, balance);

            // Assert that this address has a specific balance at the specific block
            balance = await _vechainClient.GetContractBalance("0x9c6e62B3334294D70c8e410941f52D482557955B", "0x71bdd2521C3BA14EBf41aC7f3F4Fb0b7EB1EFbd4", "2590259");
            Assert.Equal(34.985m, balance);

            // Assert that the address had no balance at genesis
            balance = await _vechainClient.GetContractBalance("0x9c6e62B3334294D70c8e410941f52D482557955B", "0x71bdd2521C3BA14EBf41aC7f3F4Fb0b7EB1EFbd4", "0");
            Assert.Equal(0m, balance);
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
                transactions = new List<string>()
            };

            var block = await _vechainClient.GetBlock("0");

            Assert.Equal(genesis, block);
        }
    }
}