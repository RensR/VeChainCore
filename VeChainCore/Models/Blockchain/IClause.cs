using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    public interface IClause
    {
        [DataMember]
        string to { get; }

        [DataMember]
        decimal value { get; }

        [DataMember]
        string data { get; }
    }
}