using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Core;
using VeChainCore.Utils;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public sealed partial class ArbitraryClause : Clause
    {
        [DataMember]
        public override string to { get; }
        [DataMember]
        public override decimal value { get; }
        [DataMember]
        public override string data { get; }
    }
}