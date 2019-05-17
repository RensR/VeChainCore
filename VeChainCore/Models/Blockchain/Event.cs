using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class Event
    {
        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string[] topics { get; set; }

        [DataMember]
        public string data { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TxMeta meta { get; set; }
    }
}