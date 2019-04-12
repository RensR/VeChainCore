using Nethereum.RLP;
using Org.BouncyCastle.Math;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;

namespace VeChainCore.Models.Core
{
    public class RlpClause : IRLPElement
    {
        public Address To { get; set; }
        public IAmount Value { get; set; }
        public string Data { get; set; }

        public RlpClause(string to, string value, string data, bool precision = true)
        {
            To = new Address(to);
            Value = new VET( new BigInteger(value), precision);
            Data = data;
        }

        public byte[] RLPData => RLP.EncodeList(new []
        {
            To.RLPData,
            RLP.EncodeElement(Value.AsBytes),
            RLP.EncodeElement(Data.HexStringToByteArray())
        });
    }
}
