using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Core;
using VeChainCore.Utils;

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