using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class VetClause
    {
        [DataMember]
        public override string to { get; }

        [DataMember]
        public override decimal value { get; }

        [DataMember]
        public override string data { get; }
    }
}