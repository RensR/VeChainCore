using Nethereum.Hex.HexConvertors.Extensions;
using VeChainCore.Models.Core;
using VeChainCore.Models.Core.Abi;
using VeChainCore.Models.Extensions;
using Xunit;

namespace VeChainCoreTest
{
    public class ContractTest
    {
        [Fact]
        public void CorrectFunctionCallTest()
        {
            var ehrtTokenContract = new Contract
            {
                Name = "EHRT Token",
                AbiDefinition = AbiContractDefinition.ContractBuilder(GetEhrtTokenAbi()),
                Address = "0xf8e1faa0367298b55f57ed17f7a2ff3f5f1d1628"
            };
            
            var approveCode = ehrtTokenContract.Execute(
                    "approve",
                    "0xeE25500c0F305cc42BdcB0f95E1d3186874bc19a", 
                    new EHRT(5000, false).ToHex())
                .ToHex();
            
            Assert.Equal("095ea7b3000000000000000000000000ee25500c0f305cc42bdcb0f95e1d3186874bc19a00000000000000000000000000000000000000000000010f0cf064dd59200000", approveCode);
        }
        
        private static string GetEhrtTokenAbi()
        {
            return
                "[{constant:true,inputs:[{name:'_tokenOwner',type:'address',},],name:'balanceOf',outputs:[{name:'',type:'uint256',},],payable:false,stateMutability:'view',type:'function',},{constant:false,inputs:[{name:'_spender',type:'address',},{name:'_tokens',type:'uint256',},],name:'approve',outputs:[{name:'',type:'bool',},],payable:false,stateMutability:'nonpayable',type:'function',}]";
        }
    }
}
