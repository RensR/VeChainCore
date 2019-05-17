using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class EventCriteria
    {
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string topic0 { get; set; }
        [DataMember]
        public string topic1 { get; set; }
        [DataMember]
        public string topic2 { get; set; }
        [DataMember]
        public string topic3 { get; set; }
        [DataMember]
        public string topic4 { get; set; }
    }
}