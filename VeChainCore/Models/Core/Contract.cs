using VeChainCore.Models.Core.Abi;

namespace VeChainCore.Models.Core
{
    public class Contract
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public AbiContractDefinition AbiContractDefinition { get; set; }
    }
}
