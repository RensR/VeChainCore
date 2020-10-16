using Nethereum.ABI;

namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public class AbiInputParameter
    {
        public dynamic Value { get; set; }
        
        public readonly IAbiParameterDefinition Definition;
        public string Name { get; }
        
        private ABIType _nethAbiType;
        
        public AbiInputParameter(string abiType, dynamic value, string name = "")
        {
            _nethAbiType = ABIType.CreateABIType(abiType);
            Definition = new AbiParameterDefinition(_nethAbiType.Name,name);
            Value = value;
            Name = name;
        }

        public AbiInputParameter(IAbiParameterDefinition definition, dynamic value, string name = "")
        {
            _nethAbiType = ABIType.CreateABIType(definition.AbiType);
            Definition = definition;
            Value = value;
            Name = name;
        }
    }
}