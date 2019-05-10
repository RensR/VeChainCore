using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;
using VeChainCore.Models.Blockchain;
using VeChainCore.Utils.Json;

namespace VeChainCore.Models.Core
{
    [DataContract]
    public class BatchExecuteRequest
    {
        [DataMember]
        public IEnumerable<Clause> clauses { get; set; }

        [DataMember]
        public string caller { get; set; }

        [DataMember]
        [JsonFormatter(typeof(VeChainHexFormatter), false)]
        public ulong? gas { get; set; }

        [DataMember]
        [JsonFormatter(typeof(VeChainHexFormatter), false)]
        public ulong? gasPrice { get; set; }
    }
}