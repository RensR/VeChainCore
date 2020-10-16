using System;
using System.Runtime.CompilerServices;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils.Cryptography;

namespace VeChainCore.Models.Core
{
    public class ECKeyPair
    {
        public const int PrivateKeySize = 32;
        public const int PublicKeySize = 64;

        private static readonly X9ECParameters CurveParams = CustomNamedCurves.GetByName("secp256k1");
        public static BigInteger HalfCurveOrder = CurveParams.N.ShiftRight(1);

        public static readonly ECDomainParameters Curve = new ECDomainParameters(
            CurveParams.Curve, CurveParams.G, CurveParams.N, CurveParams.H);


        private static readonly SecureRandom SecureRandom = new SecureRandom();

        private static readonly ECDomainParameters Domain = new ECDomainParameters(Curve.Curve,
            Curve.G, Curve.N, Curve.H);

        public BigInteger PrivateKey { get; }
        public BigInteger PublicKey { get; }


        static ECPoint GetCorrespondingPublicKey(BigInteger d)
        {
            return new FixedPointCombMultiplier().Multiply(Domain.G, d);
        }

        public ECKeyPair(BigInteger privateKey)
        {
            PrivateKey = privateKey;

            PublicKey = ECDSASign.PublicKeyFromPrivate(privateKey);
        }

        public ECKeyPair(BigInteger privateKey, BigInteger publicKey, bool check = false)
        {
            PrivateKey = privateKey;

            PublicKey = publicKey;

            if (check && !Equals(publicKey, ECDSASign.PublicKeyFromPrivate(privateKey)))
                throw new InvalidKeyException("The public key does not match the private key.");
        }


        public byte[] GetRawAddress()
        {
            var hash = Hash.Keccak256(PublicKey.ToByteArrayUnsigned().PadLeading(PublicKeySize));
            var address = new byte[20];
            Unsafe.CopyBlock(ref address[0], ref hash[12], (uint) address.Length);
            // Array.Copy(hash, 12, address, 0, address.Length);
            return address; // right most 160 bits
        }

        public string GetHexAddress()
        {
            var addressBytes = GetRawAddress();
            return addressBytes.ToHex(true);
        }

        /**
         * Sign a hash with the private key of this key pair.
         * @param message   the hash to sign
         * @return  An {@link ECDSASignature} of the hash
         */
        public ECDSASignature Sign(byte[] message)
        {
            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));

            var privateKey = new ECPrivateKeyParameters(PrivateKey, Curve);
            signer.Init(true, privateKey);
            var components = signer.GenerateSignature(message);

            return new ECDSASignature(components[0], components[1]).Canonicalize();
        }

        public static ECKeyPair Create(BigInteger privateKey)
        {
            return new ECKeyPair(privateKey, ECDSASign.PublicKeyFromPrivate(privateKey));
        }

        public static ECKeyPair Create(string privateKeyHex)
        {
            return Create(privateKeyHex.HexToByteArray());
        }

        public static ECKeyPair Create(byte[] privateKey)
        {
            if (privateKey.Length != PrivateKeySize)
                throw new ArgumentException("Invalid private key size", nameof(privateKey));

            return Create(new BigInteger(1, privateKey));
        }

        public static ECKeyPair Create()
        {
            var generator = new ECKeyPairGenerator();
            generator.Init(new ECKeyGenerationParameters(Domain, SecureRandom));
            AsymmetricCipherKeyPair keyPair = generator.GenerateKeyPair();
            var privateParams = (ECPrivateKeyParameters) keyPair.Private;
            return Create(privateParams.D);
        }

        public override bool Equals(object o)
        {
            if (this == o)
                return true;

            if (!(o is ECKeyPair))
                return false;

            var ecKeyPair = (ECKeyPair) o;

            if (!PrivateKey?.Equals(ecKeyPair.PrivateKey) ?? ecKeyPair.PrivateKey != null)
                return false;

            //return _publicKey?.Equals(ecKeyPair._publicKey) ?? ecKeyPair._publicKey == null;
            return true;
        }

        public override int GetHashCode()
        {
            int result = PrivateKey?.GetHashCode() ?? 0;
            //result = 31 * result + (_publicKey?.GetHashCode() ?? 0);
            return result;
        }
    }
}