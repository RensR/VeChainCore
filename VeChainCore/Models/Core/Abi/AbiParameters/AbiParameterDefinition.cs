namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public class AbiParameterDefinition: IAbiParameterDefinition
    {
        public string Name { get; }
        public string AbiType { get; }

        public AbiParameterDefinition(string name, string type)
        {
            Name = name;
            AbiType = type;
        }
    }
}