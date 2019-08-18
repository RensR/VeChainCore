using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public string balance { get; set; }
        [DataMember]
        public string energy { get; set; }
        [DataMember]
        public bool hasCode { get; set; }
    }
}
