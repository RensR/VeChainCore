using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VeChainCore.Models.Core.Abi.AbiParameters;

namespace VeChainCore.Models.Core.Abi
{
    public class AbiContractDefinition
    {
        public AbiConstructorDefinition Constructor { get; private set; }
        public AbiFunctionDefinition[] Functions { get; private set; }

        public static AbiContractDefinition ContractBuilder(string abiString)
        {
            return ContractBuilder(JsonConvert.DeserializeObject<JArray>(abiString));
        }
        
        public static AbiContractDefinition ContractBuilder(JArray abiJson)
        {
            var definition = new AbiContractDefinition();

            var constructorJson = abiJson.FirstOrDefault(item => item["type"].ToString() == "constructor");
            if(constructorJson != null)
            {
                definition.Constructor = ConstructorBuilder(constructorJson);
            }

            var functions = abiJson.Where(item => item["type"].ToString() == "function");
            definition.Functions = functions.Select(AbiFunctionDefinition.Builder).ToArray();

            var events = abiJson.Where(item => item["type"].ToString() == "event");
            //definition.Events = events.Select(AbiEventDefinition.Builder).ToArray();

            return definition;
        }
        
        private static AbiConstructorDefinition ConstructorBuilder(JToken abiJson)
        {
            var definition = new AbiConstructorDefinition {Payable = (bool) abiJson["payable"]};

            Enum.TryParse<AbiStateMutability>(abiJson["stateMutability"].ToString(), true, out var stateMutability);
            definition.StateMutability = stateMutability;
            
            definition.Type = "constructor";
            definition.Inputs = AbiParameterBuilder.Builder(abiJson["inputs"].ToString());
            
            return definition;
        }
    }
}
