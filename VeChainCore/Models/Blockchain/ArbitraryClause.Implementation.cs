using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Core;
using VeChainCore.Utils;

namespace VeChainCore.Models.Blockchain
{
    public sealed partial class ArbitraryClause
    {
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