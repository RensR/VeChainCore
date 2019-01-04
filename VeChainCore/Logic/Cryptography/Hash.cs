using Isopoh.Cryptography.Blake2b;

namespace VeChainCore.Logic.Cryptography
{
    public static class Hash
    {
        public static byte[] HashBlake2B(byte[] data)
        {
            return Blake2B.ComputeHash(data, null);
        }
    }
}
