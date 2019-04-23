using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class VetClause
    {
        // implementation handles serialization
        
        [DataMember]
        public string to { get; set; }

        [DataMember]
        public decimal value { get; set; }

        [DataMember]
        public string data { get; set; }

    }
}