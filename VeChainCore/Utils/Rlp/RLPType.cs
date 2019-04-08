﻿using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Math;


namespace VeChainCore.Utils.Rlp
{
    public interface IRlpType { }

    public class RlpString : IRlpType
    {
        public static byte[] EMPTY = { };

        private readonly byte[] _value;

        private RlpString(byte[] value)
        {
            _value = value;
        }

        public byte[] GetBytes()
        {
            return _value;
        }

        public BigInteger AsBigInteger()
        {
            return _value.Length == 0 ? BigInteger.Zero : new BigInteger(_value);
        }

        public override string ToString()
        {
            return _value.ByteArrayToString();
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
            if (value.SignValue < 1)
            {
                return new RlpString(EMPTY);
            }
            var bytes = value.BigIntegerToBytes();
            return new RlpString(bytes.SkipWhile(element => element == 0).ToArray());
        }

        public static RlpString Create(ulong value)
        {
            return Create(new BigInteger(value.ToString()));
        }

        public static RlpString Create(string value)
        {
            return new RlpString(value.StringToByteArray());
        }

        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            
            if (!(o is RlpString))
                return false;
            
            return _value.SequenceEqual(((RlpString)o)._value);
        }

        protected bool Equals(RlpString other)
        {
            return Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }
    }


    public class RlpList : List<IRlpType>, IRlpType
    {
        public IRlpType[] RlpData { get; set; }

        public RlpList() {}
    }
}
