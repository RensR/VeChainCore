using System;
using Org.BouncyCastle.Math;

namespace VeChainCore.Utils
{
    public static class NumberConversionExtensions
    {
        public static BigInteger ToBigInteger(this byte[] bytes)
            => new BigInteger(1, bytes);

        public static BigInteger ToBigInteger(this decimal dec)
        {
            var bits = decimal.GetBits(dec);

            var places = (bits[3] >> 16) & 0x7F;

            var div = BigInteger.Ten.Pow(places);

            var num = BigInteger.Zero;

            var signed = dec < 0;

            var high = BigInteger.ValueOf(unchecked((uint) bits[2]));
            var med = BigInteger.ValueOf(unchecked((uint) bits[1]));
            var low = BigInteger.ValueOf(unchecked((uint) bits[0]));

            num = num
                .Add(high)
                .ShiftLeft(32)
                .Add(med)
                .ShiftLeft(32)
                .Add(low)
                .Divide(div);

            if (signed)
                num = num.Negate();

            return num;
        }

        public static decimal ToDecimal(this BigInteger bi, bool noThrow = false)
        {
            var thirtyTwoBits = BigInteger.ValueOf(0xFFFFFFFFL);

            if (bi.SignValue == 0)
                return decimal.Zero;

            var signed = bi.SignValue < 0;

            if (signed)
                bi = bi.Negate();

            var low = unchecked((int) (uint) bi.And(thirtyTwoBits).LongValue);
            bi = bi.ShiftRight(32);
            var med = unchecked((int) (uint) bi.And(thirtyTwoBits).LongValue);
            bi = bi.ShiftRight(32);
            var high = unchecked((int) (uint) bi.And(thirtyTwoBits).LongValue);
            bi = bi.ShiftRight(32);
            if (!noThrow && bi.SignValue != 0)
                throw new OverflowException("BigInteger too big to conform to decimal.");

            return new decimal(low, med, high, signed, 0);
        }
    }
}