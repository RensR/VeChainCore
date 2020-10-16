using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VeChainCore.Models.Core.Abi.AbiParameters
{
    public static class AbiParameterBuilder
    {
        public static IAbiParameterDefinition[] Builder(string abiString)
        {
            var abiJson = JsonConvert.DeserializeObject<JArray>(abiString);
            return Builder(abiJson);
        }
        
        public static IAbiParameterDefinition[] Builder(JArray abiJson)
        {
            return abiJson.Select(item => 
                new AbiParameterDefinition(
                    item["name"].ToString(), 
                    item["type"].ToString()))
                .Cast<IAbiParameterDefinition>()
                .ToArray();
        }
    }
}