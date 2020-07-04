using System.Runtime.Serialization;
using Nethereum.RLP;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public abstract partial class Clause : IClause, IRLPElement
    {
        // implementation handles serialization
        string IClause.to => GetTo();

        decimal IClause.value => GetValue();

        string IClause.data => GetData();
    }
}