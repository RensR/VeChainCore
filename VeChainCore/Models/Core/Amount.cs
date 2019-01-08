using System;
using System.Numerics;

namespace VeChainCore.Models.Transaction
{
    public interface IAmount
    {
        int Precision { get; }
        byte[] AsBytes { get; }
        BigInteger AsBigInt { get; }
        string ContractAddress { get; }
    }



    public class VET : IAmount
    {
        public int Precision => 18;
        public byte[] AsBytes { get; }

        public BigInteger AsBigInt { get; }

        public string ContractAddress => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="precision">Indicate whether the precision is already taken into account</param>
        public VET(BigInteger amount, bool precision = true)
        {
            if(precision)
                AsBigInt = amount;
            else
                AsBigInt = amount * (BigInteger) Math.Pow(10, Precision);
        }
    }
}
