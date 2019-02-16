using System;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;

namespace VeChainCore.Models.Core
{
    public class ECKeyPair
    {
        public static int PrivateKeySize = 32;
        private const int PublicKeySize = 64;

        private static readonly X9ECParameters CurveParams = CustomNamedCurves.GetByName("secp256k1");
        public static BigInteger HalfCurveOrder = CurveParams.N.ShiftRight(1);

        public static ECDomainParameters Curve = new ECDomainParameters(
            CurveParams.Curve, CurveParams.G, CurveParams.N, CurveParams.H);


        private static readonly SecureRandom SecureRandom = new SecureRandom();

        private static readonly ECDomainParameters Domain = new ECDomainParameters(Curve.Curve,
            Curve.G, Curve.N, Curve.H);

        private readonly BigInteger _privateKey;
        private readonly BigInteger _publicKey;


        public ECKeyPair(BigInteger privateKey, BigInteger publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        public BigInteger GetPrivateKey()
        {
            return _privateKey;
        }

        public byte[] GetRawPrivateKey()
        {
            return Hex.ToBytesPadded(_privateKey, PrivateKeySize);
        }


        public BigInteger GetPublicKey()
        {
            return _publicKey;
        }

        public byte[] GetRawPublicKey()
        {
            return Hex.ToBytesPadded(_publicKey, PublicKeySize);
        }

        public byte[] GetRawAddress()
        {
            var hash = Hash.Keccac256(GetRawPublicKey());
            var address = new byte[20];
            Array.Copy(hash, 12, address, 0, address.Length);
            return address; // right most 160 bits
        }

        public string GetHexAddress()
        {
            var addressBytes = GetRawAddress();
            return addressBytes.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);
        }

        /**
         * Sign a hash with the private key of this key pair.
         * @param message   the hash to sign
         * @return  An {@link ECDSASignature} of the hash
         */
        public ECDSASignature Sign(byte[] message)
        {
            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));

            var privateKey = new ECPrivateKeyParameters(_privateKey, Curve);
            signer.Init(true, privateKey);
            var components = signer.GenerateSignature(message);

            return new ECDSASignature(components[0], components[1]).ToCanonicalised();
        }

        public static ECKeyPair Create(BigInteger privateKey)
        {
            return new ECKeyPair(privateKey, ECDSASign.PublicKeyFromPrivate(privateKey));
        }

        public static ECKeyPair Create(String privateKeyHex)
        {
            return Create(privateKeyHex.HexStringToByteArray());
        }

        public static ECKeyPair Create(byte[] privateKey)
        {
            if (privateKey.Length == PrivateKeySize)
            {
                return Create(Hex.BytesToBigInt(privateKey));
            }
            throw new Exception("Invalid privatekey size");
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
            
            if (o == null || !(o is ECKeyPair))
                return false;

            var ecKeyPair = (ECKeyPair) o;

            if (!_privateKey?.Equals(ecKeyPair._privateKey) ?? ecKeyPair._privateKey != null)
                return false;
            
            return _publicKey?.Equals(ecKeyPair._publicKey) ?? ecKeyPair._publicKey == null;
        }

        public override int GetHashCode()
        {
            var result = _privateKey != null ? _privateKey.GetHashCode() : 0;
            result = 31 * result + (_publicKey != null ? _publicKey.GetHashCode() : 0);
            return result;
        }

    }
}
