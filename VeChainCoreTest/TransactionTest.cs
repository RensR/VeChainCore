using System.Globalization;
using VeChainCore.Client;
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
        public void CreateTransaction()
        {
            var realTransaction = new Transaction{
                chainTag= 1,
                blockRef = ulong.Parse("0x00000000aabbccdd", NumberStyles.HexNumber),
                expiration= 32,
                clauses= new[]{ new Clause{
                    to= "0x7567d83b7b8d80addcb281a71d54fc7b3364ffed",
                    value= "10000",
                    data= "0x000000606060"}
                , new Clause
                    {
                    to= "0x7567d83b7b8d80addcb281a71d54fc7b3364ffed",
                    value= "20000",
                    data= "0x000000606060"
                }},
                gasPriceCoef= 128,
                gas= 21000,
                dependsOn= null,
                nonce= "12345678"
            };

            var rawWithoutMeta =
                "2a1c25ce0d66f45276a5f308b99bf410e2fc7d5b6ea37a49f2ab9f1da9446478";



            //var txid = rawTransaction.CalculateTxId("0xeCC159751F9aed21399d5e3CE72BC9D4FcCB9cCc");
           

            //var asHexString = RlpEncoder.Encode(encoded);

            //var hash = Hash.HashBlake2B(asHexString);
            //var hashString = hash.ToHex();

            //Assert.Equal(rawWithoutMeta, hash.ToString());
        }
    }
}
