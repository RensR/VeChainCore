using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class Output
    {
        [DataMember]
        public string contractAddress { get; set; }

        [DataMember]
        public List<Event> events { get; set; }

        [DataMember]
        public List<Transfer> transfers { get; set; }
    }
}