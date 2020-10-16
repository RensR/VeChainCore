using Nethereum.ABI;

namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public class AbiOutputParameter
    {
        public dynamic Result { get; set; }
        
        public IAbiParameterDefinition Definition;
        public string Name { get; }
        
        private ABIType _nethAbiType;

        public AbiOutputParameter(string abiType, string name = "")
        {
            _nethAbiType = ABIType.CreateABIType(abiType);
        }

        public AbiOutputParameter(IAbiParameterDefinition definition, string name = "")
        {
            _nethAbiType = ABIType.CreateABIType(definition.AbiType);
            Name = name;
        }
    }
}