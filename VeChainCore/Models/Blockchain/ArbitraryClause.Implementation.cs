using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;

namespace VeChainCore.Models.Blockchain
{
    public sealed partial class ArbitraryClause : Clause
    {
        public ArbitraryClause(string to, decimal value, string data)
            => (this.to, this.value, this.data) = (to, value, data);


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

        protected override string GetTo() => to;

        protected override decimal GetValue() => value;

        protected override string GetData() => data;
    }
}