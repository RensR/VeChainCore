using System.Runtime.Serialization;
using Nethereum.RLP;
using Utf8Json;
using VeChainCore.Utils.Json;

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