using Org.BouncyCastle.Math;

namespace VeChainCore.Models.Core
{
    public class RawClause
    {
        public Address To { get; set; }
        public IAmount Value { get; set; }
        public string Data { get; set; }

        public RawClause(string to, string value, string data, bool precision = true)
        {
            To = new Address(to);
            Value = new VET( new BigInteger(value), precision);
            Data = data;
        }
    }
}
