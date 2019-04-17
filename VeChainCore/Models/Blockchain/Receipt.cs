using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class Receipt
    {
        [DataMember]
        public uint gasUsed { get; set; }

        [DataMember]
        public string gasPayer { get; set; }

        [DataMember]
        public string paid { get; set; }

        [DataMember]
        public string reward { get; set; }

        [DataMember]
        public bool reverted { get; set; }

        [DataMember]
        public List<Output> outputs { get; set; }

        [DataMember]
        public LogMeta meta { get; set; }
    }
}