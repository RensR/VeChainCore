using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;
using VeChainCore.Utils.Json;

namespace VeChainCore.Models.Blockchain
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso href="https://github.com/vechain/thor/wiki/Transaction-Model"/>
    [DataContract]
    public partial class Transaction
    {
        [DataMember]
        public Network chainTag { get; set; }

        [DataMember]
        [JsonFormatter(typeof(VeChainHexFormatter))]
        public ulong blockRef { get; set; }

        [DataMember]
        public uint expiration { get; set; }

        [DataMember]
        public List<Clause> clauses { get; set; }

        [DataMember]
        public byte gasPriceCoef { get; set; }

        [DataMember]
        [JsonFormatter(typeof(VeChainHexFormatter))]
        public ulong gas { get; set; }

        [DataMember]
        public string dependsOn { get; set; }

        [DataMember]
        [JsonFormatter(typeof(VeChainHexFormatter))]
        public ulong nonce { get; set; }

        [DataMember]
        private static readonly byte[] Reserved = {0xc0};

        [DataMember]
        public string signature { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string origin { get; set; }

        [DataMember]
        public ulong size { get; set; }

        [DataMember]
        public TxMeta meta { get; set; }
    }
}