using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class TransferCriteria
    {
        [DataMember]
        public string txOrigin { get; set; }
        [DataMember]
        public string sender { get; set; }
        [DataMember]
        public string recipient { get; set; }
    }
}