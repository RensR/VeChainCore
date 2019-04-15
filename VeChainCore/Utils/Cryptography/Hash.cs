using System;
using System.Threading;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace VeChainCore.Utils.Cryptography
{
    public static class Hash
    {
        private static readonly ThreadLocal<IDigest> TlsBlake2B
            = new ThreadLocal<IDigest>(() => new Blake2bDigest(null, 32, null, null));

        private static readonly ThreadLocal<IDigest> TlsKeccak256
            = new ThreadLocal<IDigest>(() => new KeccakDigest(256));

        private static byte[] ResetAndHash(IDigest h, byte[] data)
        {
            h.BlockUpdate(data, 0, data.Length);
            var hash = new byte[32];
            h.DoFinal(hash, 0); // .DoFinal calls .Reset
            return hash;
        }

        public static byte[] Blake2B(byte[] data)
            => ResetAndHash(TlsBlake2B.Value, data);

        public static byte[] Keccak256(byte[] data)
            => ResetAndHash(TlsKeccak256.Value, data);
    }
}