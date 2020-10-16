using System;
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
        
        public string Name { get; protected internal set; }

        public bool Constant { get; protected internal set; }

        public bool Payable { get; protected internal set; }

        public AbiStateMutability stateMutability { get; protected internal set; }

        public IAbiParameterDefinition[] inputs { get; protected internal set; }

        public IAbiParameterDefinition[] outputs { get; protected internal set; }

        public byte[] Sha3Signature { get; protected internal set; }
        
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
            definition.stateMutability = stateMutability;

            definition.inputs = new AbiParameterBuilder().Builder(abiJson["inputs"].ToString());
            definition.outputs = new AbiParameterBuilder().Builder(abiJson["outputs"].ToString());
            definition.Sha3Signature = GetNethFunctionAbi(definition).Sha3Signature.ToBytes();

            return definition;
        }
        
        private static FunctionABI GetNethFunctionAbi(AbiFunctionDefinition definition)
        {
            return new FunctionABI(definition.Name, definition.Constant)
            {
                InputParameters = GetNethParameters(definition.inputs),
                OutputParameters = GetNethParameters(definition.outputs)
            };
        }
        
        private static Parameter[] GetNethParameters(IAbiParameterDefinition[] parameters)
        {
            var result = new Parameter[parameters.Length];
            for(var index = 0; index < parameters.Length; index++)
            {
                result[index] = new Parameter(parameters[index].AbiType, parameters[index].Name, index + 1);
            }
            return result;
        }
    }
}