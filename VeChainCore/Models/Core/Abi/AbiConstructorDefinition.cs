using VeChainCore.Models.Core.Abi.AbiParameters;

namespace VeChainCore.Models.Core.Abi
{
    public class AbiConstructorDefinition
    {
        public string Type { get; protected internal set; }
        
        public bool Payable { get; protected internal set; }
        
        public AbiStateMutability StateMutability { get; protected internal set; }
        
        public IAbiParameterDefinition[] Inputs { get; protected internal set; }
    }
}