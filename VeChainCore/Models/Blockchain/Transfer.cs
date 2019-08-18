using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class Transfer
    {
        [DataMember]
        public string sender { get; set; }

        [DataMember]
        public string recipient { get; set; }

        [DataMember]
        public string amount { get; set; }
        
        [DataMember(IsRequired = false)]
        public TxMeta meta { get; set; }
    }
}