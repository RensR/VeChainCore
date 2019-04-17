using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class TransferResult
    {
        [DataMember]
        public string id { get; set; }
    }
}