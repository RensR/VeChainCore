using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class TxMeta : IEquatable<TxMeta>
    {
        [DataMember]
        public string blockID { get; set; }

        [DataMember]
        public ulong blockNumber { get; set; }

        [DataMember]
        public ulong blockTimestamp { get; set; }

        [DataMember(IsRequired = false)]
        public string txID { get; set; }

        [DataMember(IsRequired = false)]
        public string txOrigin { get; set; }
    }
}