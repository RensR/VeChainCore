using System.Collections.Generic;
using System.Linq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;

namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public class AbiParameterCoder
    {
        public static byte[] EncodeParameter(AbiInputParameter parameter)
        {
            return Encode(new[] {parameter});
        }
        public static byte[] EncodeParameter(IEnumerable<AbiInputParameter> parameters)
        {
            return Encode(parameters);
        }

        public static AbiOutputParameter DecodeParameter(IAbiParameterDefinition definition, byte[] data)
        {
            var outputs = Decode(new[]{definition}, data);
            return outputs.Length > 0 ? outputs[0] : null;
        }

        public static AbiOutputParameter[] DecodeParameter(IEnumerable<IAbiParameterDefinition> definition, byte[] data)
        {
            return Decode(definition, data);
        }

        private static byte[] Encode(IEnumerable<AbiInputParameter> parameters)
        {
            var netherParameters = new List<Parameter>();
            var values = new List<dynamic>();

            foreach (var parameter in parameters)
            {
                netherParameters.Add(new Parameter(parameter.Definition.AbiType, parameter.Definition.Name));
                values.Add(parameter.Value);
            }

            var encoder = new ParametersEncoder();
            return encoder.EncodeParameters(netherParameters.ToArray(), values.ToArray());
        }

        private static AbiOutputParameter[] Decode(IEnumerable<IAbiParameterDefinition> parameters, byte[] data)
        {
            var outputs = new ParameterDecoder().DecodeDefaultData(
                    data, 
                    parameters
                        .Select(parame => new Parameter(parame.AbiType, parame.Name))
                        .ToArray());

            return outputs.Select(output => new AbiOutputParameter(output.Parameter.Type, output.Parameter.Name) {Result = output.Result}).ToArray();
        }
    }
}