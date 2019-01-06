using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Nethereum.RLP;
using Org.BouncyCastle.Utilities;
using VeChainCore.Logic;

namespace VeChainCore.Models
{
    public interface RlpType
    {
    }

    public class RlpString : RlpType
    {
        public static byte[] EMPTY = { };

        private readonly byte[] value;

        private RlpString(byte[] value)
        {
            this.value = value;
        }

        public byte[] GetBytes()
        {
            return value;
        }

        public BigInteger AsBigInteger()
        {
            return value.Length == 0 ? BigInteger.Zero : new BigInteger(value);
        }

        public override string ToString()
        {
            return "0x" + Hex.ByteArrayToString(value);
        }

        public static RlpString Create(byte[] value)
        {
            return new RlpString(value);
        }

        public static RlpString Create(byte value)
        {
            return new RlpString(new[] {value});
        }

        public static RlpString Create(BigInteger value)
        {
            // RLP encoding only supports positive integer values
            if (value.Sign < 1)
            {
                return new RlpString(EMPTY);
            }
            var bytes = value.ToBytesForRLPEncoding();
            return new RlpString(bytes.SkipWhile(element => element == 0).ToArray());
        }

        public static RlpString Create(long value)
        {
            return Create((BigInteger) value);
        }

        public static RlpString Create(string value)
        {
            return new RlpString(value.ToBytesForRLPEncoding());
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is RlpString))
            {
                return false;
            }

            var rlpString = (RlpString) o;

            return Arrays.AreEqual(value, rlpString.value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /**
    * RLP list type.
    */
    public class RlpList : RlpType
    {
        private readonly List<RlpType> _values;

        public RlpList(List<RlpType> values)
        {
            _values = values;
        }

        public List<RlpType> GetValues()
        {
            return _values;
        }
    }
}
