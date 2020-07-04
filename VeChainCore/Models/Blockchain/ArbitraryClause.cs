using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public sealed partial class ArbitraryClause
    {
        [DataMember]
        public string to { get; }

        [DataMember]
        public decimal value { get; }

        [DataMember]
        public string data { get; }
    }
}