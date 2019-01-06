using Isopoh.Cryptography.Blake2b;

namespace VeChainCore.Logic.Cryptography
{
    public static class Hash
    {
        public static Blake2BConfig Config = new Blake2BConfig{OutputSizeInBytes = 32 };

        public static byte[] HashBlake2B(byte[] data)
        {
            return Blake2B.ComputeHash(data, Config, null);
        }
    }
}
