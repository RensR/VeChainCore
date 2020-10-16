using System;
using System.IO;
using VeChainCore.Models.Core.Abi.AbiParameters;
using VeChainCore.Models.Extensions;

namespace VeChainCore.Models.Core.Abi
{
    public class AbiFunctionCoder
    {
        private readonly AbiFunctionDefinition _definition;
        
        public AbiFunctionCoder(string abiJson)
        {
            _definition = AbiFunctionDefinition.Builder(abiJson);
        }

        public AbiFunctionCoder(AbiFunctionDefinition definition)
        {
            _definition = definition;
        }

        public byte[] Encode(params dynamic[] values)
        {
            var stream = new MemoryStream();
            if(values.Length == _definition.Inputs.Length)
            {
                try
                {
                    var parameters = new AbiInputParameter[values.Length];
                    for(var index = 0; index < values.Length; index++)
                    {
                        parameters[index] = new AbiInputParameter(_definition.Inputs[index], values[index]);
                    }
                    stream.Append(_definition.Sha3Signature);
                    stream.Append(AbiParameterCoder.EncodeParameter(parameters));
                }
                catch
                {
                    throw new ArgumentException("input values invalid");
                }
            }
            else
            {
                throw new ArgumentException("values number not match");
            }
            return stream.ToArray();
        }

        public AbiOutputParameter[] Decode(byte[] outputData)
        {  
            return AbiParameterCoder.DecodeParameter(_definition.Outputs, outputData);
        }
    }
}