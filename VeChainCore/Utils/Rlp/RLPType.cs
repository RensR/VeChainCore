using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VeChainCore.Utils.Rlp
{
    public interface IRlpType
    {
    }

    public class RlpString : IRlpType
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
            return "0x" + value.ByteArrayToString();
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
            var bytes = value.BigIntegerToBytes();
            return new RlpString(bytes.SkipWhile(element => element == 0).ToArray());
        }

        public static RlpString Create(long value)
        {
            return Create((BigInteger) value);
        }

        public static RlpString Create(string value)
        {
            return new RlpString(value.StringToByteArray());
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

            return value.SequenceEqual(rlpString.value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /**
    * RLP list type.
    */
    public class RlpList : IRlpType
    {
        private readonly List<IRlpType> _values;

        public RlpList(List<IRlpType> values)
        {
            _values = values;
        }

        public List<IRlpType> GetValues()
        {
            return _values;
        }
    }
}
