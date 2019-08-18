using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;
using VeChainCore.Client;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class Block : IEquatable<Block>
    {
        [DataMember]
        public ulong number { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public uint size { get; set; }

        [DataMember]
        public string parentID { get; set; }

        [DataMember]
        public ulong timestamp { get; set; }

        [DataMember]
        public uint gasLimit { get; set; }

        [DataMember]
        public string beneficiary { get; set; }

        [DataMember]
        public uint gasUsed { get; set; }

        [DataMember]
        public uint totalScore { get; set; }

        [DataMember]
        public string txsRoot { get; set; }

        [DataMember]
        public string stateRoot { get; set; }

        [DataMember]
        public string receiptsRoot { get; set; }

        [DataMember]
        public string signer { get; set; }

        [DataMember]
        public bool isTrunk { get; set; }

        [DataMember]
        public List<string> transactions { get; set; }
    }
}