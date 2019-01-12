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
            this._privateKey = privateKey;
            this._publicKey = publicKey;
        }

        public BigInteger getPrivateKey()
        {
            return _privateKey;
        }

        public byte[] getRawPrivateKey()
        {
            return Hex.ToBytesPadded(_privateKey, PrivateKeySize);
        }


        public BigInteger getPublicKey()
        {
            return _publicKey;
        }

        public byte[] getRawPublicKey()
        {
            return Hex.ToBytesPadded(_publicKey, PublicKeySize);
        }

        public byte[] getRawAddress()
        {
            byte[] hash = Hash.Keccac256(getRawPublicKey());
            byte[] address = new byte[20];
            Array.Copy(hash, 12, address, 0, address.Length);
            return address; // right most 160 bits
        }

        public string GetHexAddress()
        {
            var addressBytes = getRawAddress();
            return addressBytes.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);
        }

        /**
         * Sign a hash with the private key of this key pair.
         * @param message   the hash to sign
         * @return  An {@link ECDSASignature} of the hash
         */
        public ECDSASignature Sign(byte[] message)
        {
            ECDsaSigner signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));

            ECPrivateKeyParameters privKey = new ECPrivateKeyParameters(_privateKey, Curve);
            signer.Init(true, privKey);
            BigInteger[] components = signer.GenerateSignature(message);

            return new ECDSASignature(components[0], components[1]).ToCanonicalised();
        }

        public static ECKeyPair create(BigInteger privateKey)
        {
            return new ECKeyPair(privateKey, ECDSASign.PublicKeyFromPrivate(privateKey));
        }

        public static ECKeyPair create(String privateKeyHex)
        {
            byte[] privKey = privateKeyHex.HexStringToByteArray();
            return create(privKey);
        }

        public static ECKeyPair create(byte[] privateKey)
        {
            if (privateKey.Length == PrivateKeySize)
            {
                return create(Hex.BytesToBigInt(privateKey));
            }
            throw new Exception("Invalid privatekey size");
        }

        public static ECKeyPair create()
        {
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            ECKeyGenerationParameters keygenParams = new ECKeyGenerationParameters(Domain,
                SecureRandom);
            generator.Init(keygenParams);
            AsymmetricCipherKeyPair keypair = generator.GenerateKeyPair();
            ECPrivateKeyParameters privParams = (ECPrivateKeyParameters) keypair.Private;
            ECKeyPair k = ECKeyPair.create(privParams.D);
            return k;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !(o is ECKeyPair))
            {
                return false;
            }

            ECKeyPair ecKeyPair = (ECKeyPair) o;

            if (!_privateKey?.Equals(ecKeyPair._privateKey) ?? ecKeyPair._privateKey != null)
            {
                return false;
            }

            return _publicKey?.Equals(ecKeyPair._publicKey) ?? ecKeyPair._publicKey == null;
        }

        public override int GetHashCode()
        {
            int result = _privateKey != null ? _privateKey.GetHashCode() : 0;
            result = 31 * result + (_publicKey != null ? _publicKey.GetHashCode() : 0);
            return result;
        }

    }
}
