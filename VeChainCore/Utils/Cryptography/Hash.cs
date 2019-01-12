using Org.BouncyCastle.Crypto.Digests;

namespace VeChainCore.Utils.Cryptography
{
    public static class Hash
    {
        public static byte[] HashBlake2B(byte[] data)
        {
            Blake2bDigest h = new Blake2bDigest(null, 32, null, null);
            h.BlockUpdate(data, 0, data.Length);
            var hash = new byte[32];
            h.DoFinal(hash, 0);
            return hash;
        }

        public static byte[] Keccac256(byte[] data)
        {
            KeccakDigest k = new KeccakDigest(256);
            k.BlockUpdate(data, 0, data.Length);
            var hash = new byte[32];
            k.DoFinal(hash, 0);
            return hash;
        }
    }
}
