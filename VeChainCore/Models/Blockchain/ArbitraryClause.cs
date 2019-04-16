using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;

namespace VeChainCore.Models.Blockchain
{
    public sealed class ArbitraryClause : Clause
    {
        public override string to { get; }
        public override decimal value { get; }
        public override string data { get; }


        [IgnoreDataMember]
        public byte[][] RlpDataParts
            => new[]
            {
                new Address(to).RLPData,
                RLP.EncodeElement(value.ToBigInteger().ToByteArrayUnsigned()),
                RLP.EncodeElement(data.HexToByteArray())
            };

        [IgnoreDataMember]
        public override byte[] RLPData => RLP.EncodeList(RlpDataParts);
    }
}