﻿using System;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities;
using VeChainCore.Models.Core;

namespace VeChainCore.Utils.Cryptography
{
    public class ECDSASign
    {
        public static SignatureData SignMessage(byte[] message, ECKeyPair keys, bool needToHash)
        {
            BigInteger publicKey = keys.GetPublicKey();
            var messageHash = needToHash ? Hash.HashBlake2B(message) : message;
            var recId = -1;

            ECDSASignature sig = keys.Sign(messageHash);
            for (int i = 0; i < 4; i++)
            {
                BigInteger k = RecoverFromSignature(i, sig, messageHash);            

                if (k != null && k.Equals(publicKey))
                {
                    recId = i;
                    break;
                }
            }

            if (recId == -1)
                throw new Exception("Sign the data failed.");
            if (recId == 2 || recId == 3) throw new Exception("Recovery is not valid for VeChain MainNet.");

            byte v = (byte) recId;
            sig.R.ToByteArray();
            byte[] r = BigIntToBytesWithPadding(sig.R, 32);
            byte[] s = BigIntToBytesWithPadding(sig.S, 32);

            return new SignatureData(v, r, s);
        }

        public static byte[] BigIntToBytesWithPadding(BigInteger bigint, int size)
        {
            byte[] asBytes = bigint.BigIntegerToBytes();
            var newArray = new byte[size];

            var startAt = newArray.Length - asBytes.Length;
            Array.Copy(asBytes, 0, newArray, startAt, asBytes.Length);

            return newArray;
        }

        /**
    * Recover the public key from signature and message.
    * @param recId recovery id which 0 or 1.
    * @param sig {@link ECDSASignature} a signature object
    * @param message message bytes array.
    * @return public key represented by {@link BigInteger}
    */
        public static BigInteger RecoverFromSignature(int recId, ECDSASignature sig, byte[] message)
        {
            if (!(recId == 0 || recId == 1))
                throw new Exception("recId must be 0 or 1");
            if (message == null)
                throw new Exception("message cannot be null");


            BigInteger n = ECKeyPair.Curve.N;  // Curve order.
            BigInteger i = BigInteger.ValueOf((long)recId / 2);
            BigInteger x = sig.R.Add(i.Multiply(n));

            // TODO SecP256K1Curve
            //BigInteger prime = SecP256K1Curve.q;
            //if (x.CompareTo(prime) >= 0)
            //{
            //    return null;
            //}

            ECPoint R = DecompressKey(x, (recId & 1) == 1);

            if (!R.Multiply(n).IsInfinity)
            {
                return null;
            }

            BigInteger e = new BigInteger(1, message);

            BigInteger eInv = BigInteger.Zero.Subtract(e).Mod(n);
            BigInteger rInv = sig.R.ModInverse(n);
            BigInteger srInv = rInv.Multiply(sig.S).Mod(n);
            BigInteger eInvrInv = rInv.Multiply(eInv).Mod(n);
            ECPoint q = ECAlgorithms.SumOfTwoMultiplies(ECKeyPair.Curve.G, eInvrInv, R, srInv);

            byte[] qBytes = q.GetEncoded(false);
            // We remove the prefix
            return new BigInteger(1, Arrays.CopyOfRange(qBytes, 1, qBytes.Length));
        }

        private static ECPoint DecompressKey(BigInteger xBN, bool yBit)
        {
            byte[] compEnc = X9IntegerConverter.IntegerToBytes(xBN, 1 + X9IntegerConverter.GetByteLength(ECKeyPair.Curve.Curve));
            compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
            return ECKeyPair.Curve.Curve.DecodePoint(compEnc);
        }

        public static BigInteger PublicKeyFromPrivate(BigInteger privKey)
        {
            ECPoint point = PublicPointFromPrivate(privKey);

            byte[] encoded = point.GetEncoded(false);
            return new BigInteger(1, Arrays.CopyOfRange(encoded, 1, encoded.Length));  // remove prefix
        }

        public static ECPoint PublicPointFromPrivate(BigInteger privKey)
        {
            /*
             * TODO: FixedPointCombMultiplier currently doesn't support scalars longer than the group
             * order, but that could change in future versions.
             */
            if (privKey.BitLength > ECKeyPair.Curve.N.BitLength)
            {
                privKey = privKey.Mod(ECKeyPair.Curve.N);
            }
            return new FixedPointCombMultiplier().Multiply(ECKeyPair.Curve.G, privKey);
        }
    }


    public class SignatureData
    {
        public byte V;
        public byte[] R;
        public byte[] S;

        public SignatureData(byte v, byte[] r, byte[] s)
        {
            V = v;
            R = r;
            S = s;
        }

        public override bool Equals(object o)
        {
            if (!(o is SignatureData that)) return false;
            if (V != that.V)
            {
                return false;
            }
            return Equals(R, that.R) && Equals(S, that.S);
        }

        protected bool Equals(SignatureData other)
        {
            return V == other.V && Equals(R, other.R) && Equals(S, other.S);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = V.GetHashCode();
                hashCode = (hashCode * 397) ^ (R != null ? R.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (S != null ? S.GetHashCode() : 0);
                return hashCode;
            }
        }

        public byte[] ToByteArray()
        {
            int size = R.Length + S.Length + 1;
            byte[] flat = new byte[size];
            Array.Copy(R, 0, flat, 0, R.Length);
            Array.Copy(S, 0, flat, R.Length, S.Length);
            flat[size - 1] = V;
            return flat;
        }
    }
}