using System;
using System.Collections.Generic;
using Nethereum.ABI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VeChainCore.Models.Core.Abi.AbiParameters;
using VeChainCore.Models.Extensions;

namespace VeChainCore.Models.Core.Abi
{
    public class AbiFunctionDefinition
    {
        public string Type => "function";
        
        public string Name { get; private set; }

        public bool Constant { get; protected internal set; }

        public bool Payable { get; protected internal set; }

        public AbiStateMutability StateMutability { get; protected internal set; }

        public IAbiParameterDefinition[] Inputs { get; private set; }

        public IAbiParameterDefinition[] Outputs { get; private set; }

        public byte[] Sha3Signature { get; private set; }
        
        public static AbiFunctionDefinition Builder(string abiString)
        {
            var abiJson = JsonConvert.DeserializeObject<JToken>(abiString);
            return Builder(abiJson);
        }
        
        public static AbiFunctionDefinition Builder(JToken abiJson)
        {
            var definition = new AbiFunctionDefinition
            {
                Name = abiJson["name"].ToString(),
                Constant = (bool) abiJson["constant"],
                Payable = (bool) abiJson["payable"] 
            };
            
            Enum.TryParse(abiJson["stateMutability"].ToString(), true,
                out AbiStateMutability stateMutability);
            definition.StateMutability = stateMutability;

            definition.Inputs = new AbiParameterBuilder().Builder(abiJson["inputs"].ToString());
            definition.Outputs = new AbiParameterBuilder().Builder(abiJson["outputs"].ToString());
            definition.Sha3Signature = GetNethFunctionAbi(definition).Sha3Signature.ToBytes();

            return definition;
        }
        
        private static FunctionABI GetNethFunctionAbi(AbiFunctionDefinition definition)
        {
            return new FunctionABI(definition.Name, definition.Constant)
            {
                InputParameters = GetNethParameters(definition.Inputs),
                OutputParameters = GetNethParameters(definition.Outputs)
            };
        }
        
        private static Parameter[] GetNethParameters(IReadOnlyList<IAbiParameterDefinition> parameters)
        {
            var result = new Parameter[parameters.Count];
            for(var index = 0; index < parameters.Count; index++)
            {
                result[index] = new Parameter(parameters[index].AbiType, parameters[index].Name, index + 1);
            }
            return result;
        }
    }
}