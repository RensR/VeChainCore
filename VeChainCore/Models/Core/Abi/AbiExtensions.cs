using System;
using System.Linq;
using VeChainCore.Models.Core.Abi.AbiParameters;

namespace VeChainCore.Models.Core.Abi
{
    public static class AbiExtensions
    {
        public static AbiFunctionCoder GetFunctionCoder(this AbiContractDefinition definition, string funcName)
        {
            var funcDefinition = definition.Functions.FirstOrDefault(item => item.Name == funcName);
            if (funcDefinition == null)
            {
                throw new Exception($"function {funcName} does not exists");
            }

            return new AbiFunctionCoder(funcDefinition);
        }
        
        public static byte[] EncodeConstructorWithParameter(this AbiContractDefinition definition, params dynamic[] values)
        {
            if(definition.Constructor == null)
            {
                throw new ArgumentException("Constructor has no parameter");
            }

            return AbiParameterCoder
                .EncodeParameter(values.Select(t => new AbiInputParameter(definition.Constructor.Inputs[0], t))
                    .ToArray());
        }

        public static byte[] Execute(this Contract contract, string function, params dynamic[] values)
        {
            if (contract.AbiDefinition == null)
            {
                throw new ArgumentException("No abi found");
            }

            return contract.AbiDefinition.GetFunctionCoder(function).Encode(values);
        }
        
        //public static AbiEventCoder GetEventCoder(this AbiContractDefinition definition, string eventName)
        //{
        //    var eventDefinition = definition.Events.First(item => item.Name == eventName);
        //    if (eventDefinition == null)
        //    {
        //        throw new Exception("event not exists");
        //    }
//
        //    return new AbiEventCoder(eventDefinition);
        //}
    }
}