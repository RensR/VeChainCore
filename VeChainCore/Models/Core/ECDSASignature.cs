using Org.BouncyCastle.Math;
using VeChainCore.Utils;

namespace VeChainCore.Models.Core
{
    public class ECDSASignature
    {
        public BigInteger R;
        public BigInteger S;

        public ECDSASignature(byte[] rBytes, byte[] sBytes)
        {
            R = new BigInteger(1, rBytes);
            S = new BigInteger(1, sBytes);
        }


        public ECDSASignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        /**
         * @return true if the S component is "low", that means it is below
         * {@link ECKeyPair#HALF_CURVE_ORDER}. See
         * <a href="https://github.com/bitcoin/bips/blob/master/bip-0062.mediawiki#Low_S_values_in_signatures">
         * BIP62</a>.
         */
        private bool IsCanonical()
        {
            return S.CompareTo(ECKeyPair.HalfCurveOrder) <= 0;
        }

        /**
         * Will automatically adjust the S component to be less than or equal to half the curve
         * order, if necessary. This is required because for every signature (R,S) the signature
         * (R, -S (mod N)) is a valid signature of the same message. However, we dislike the
         * ability to modify the bits of a Bitcoin transaction after it'S been signed, as that
         * violates various assumed invariants. Thus in future only one of those forms will be
         * considered legal and the other will be banned.
         *
         * @return the signature in a canonicalised form.
         */

        public ECDSASignature Canonicalize()
        {
            if (!IsCanonical())
            {
                // The order of the curve is the number of valid points that exist on that curve.
                // If S is in the upper half of the number of valid points, then bring it back to
                // the lower half. Otherwise, imagine that
                //    N = 10
                //    S = 8, so (-8 % 10 == 2) thus both (R, 8) and (R, 2) are valid solutions.
                //    10 - 8 == 2, giving us always the latter solution, which is canonical.
                return new ECDSASignature(R, ECKeyPair.Curve.N.Subtract(S));
            }

            return this;
        }
    }
}