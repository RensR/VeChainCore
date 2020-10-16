namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public class AbiParameterDefinition: IAbiParameterDefinition
    {
        public string Name { get; set; }
        public string AbiType { get; set; }

        public AbiParameterDefinition(string name,string type)
        {
            Name = name;
            AbiType = type;
        }
    }
}