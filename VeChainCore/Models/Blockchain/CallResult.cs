using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class CallResult
    {
        [DataMember]
        public string data { get; set; }

        [DataMember]
        public List<Event> events { get; set; }

        [DataMember]
        public List<Transfer> transfers { get; set; }

        [DataMember]
        public ulong gasUsed { get; set; }

        [DataMember]
        public bool reverted { get; set; }

        [DataMember]
        public string vmError { get; set; }
    }
}