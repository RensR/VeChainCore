using Org.BouncyCastle.Math;

namespace VeChainCore.Models.Core
{
    public interface IAmount
    {
        int Precision { get; }
        byte[] AsBytes { get; }
        BigInteger AsBigInt { get; }
        string ContractAddress { get; }
    }
}