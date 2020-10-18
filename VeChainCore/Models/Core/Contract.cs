using VeChainCore.Models.Core.Abi;

namespace VeChainCore.Models.Core
{
    public class Contract
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public AbiContractDefinition AbiDefinition { get; set; }

        public static implicit operator string(Contract contract)
        {
            return contract.Address;
        }
    }
}
