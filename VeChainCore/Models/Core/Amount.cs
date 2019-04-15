using Org.BouncyCastle.Math;
using VeChainCore.Utils;

namespace VeChainCore.Models.Core
{
    public abstract class Amount : IAmount
    {
        protected decimal _decimalsMultiplier;

        public abstract decimal Value { get; }
        public abstract string ContractAddress { get; }

        public int Decimals { get; }
        public decimal DecimalsMultiplier => _decimalsMultiplier;

        public byte[] AsBytes => AsBigInt.ToByteArrayUnsigned();

        public BigInteger AsBigInt => (Value * _decimalsMultiplier).ToBigInteger();


        protected Amount(int decimals)
        {
            Decimals = decimals;
            _decimalsMultiplier = BigInteger.Ten.Pow(Decimals).ToDecimal();
        }
    }
}