using Nethereum.ABI;

namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public class AbiInputParameter
    {
        public dynamic Value { get; set; }
        
        public IAbiParameterDefinition Definition;
        public string Name { get; }
        
        private ABIType _nethAbiType;
        
        public AbiInputParameter(string abiType, dynamic value, string name = "")
        {
            _nethAbiType = ABIType.CreateABIType(abiType);
            Value = value;
            Name = name;
        }

        public AbiInputParameter(IAbiParameterDefinition definition, dynamic value, string name = "")
        {
            _nethAbiType = ABIType.CreateABIType(definition.AbiType);
            Value = value;
            Name = name;
        }
    }
}