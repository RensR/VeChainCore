using Org.BouncyCastle.Math;
using VeChainCore.Utils;

namespace VeChainCore.Models.Core
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

        public byte[] AsBytes => AsBigInt.BigIntegerToBytes();

        public BigInteger AsBigInt { get; }

        public string ContractAddress => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="precision">Indicate whether the precision is already taken into account</param>
        public VET(BigInteger amount, bool precision = true)
        {
            AsBigInt = precision ? amount : amount.Multiply(new BigInteger('1' + new string('\t', Precision)));
        }
    }
}
