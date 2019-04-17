using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
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
        [JsonFormatter(typeof(VeChainHexFormatter), false)]
        public ulong blockRef { get; set; }

        [DataMember]
        public uint expiration { get; set; }

        [DataMember]
        public List<Clause> clauses { get; set; }

        [DataMember]
        public byte gasPriceCoef { get; set; }

        [DataMember]
        public ulong gas { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false), DefaultValue(null)]
        public string dependsOn { get; set; }

        [DataMember, JsonFormatter(typeof(VeChainHexFormatter), true)]
        public ulong nonce { get; set; }

        [DataMember]
        private static readonly byte[] Reserved = {0xc0};

        [DataMember]
        public string signature { get; set; }

        [IgnoreDataMember]
        public string id { get; set; }

        [IgnoreDataMember]
        public string origin { get; set; }


        [IgnoreDataMember]
        public ulong size { get; set; }

        [IgnoreDataMember]
        public TxMeta meta { get; set; }
    }
}