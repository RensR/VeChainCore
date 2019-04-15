using System.Runtime.CompilerServices;

namespace VeChainCore.Utils.Cryptography
{
    public class SignatureData
    {
        public readonly byte V;
        public readonly byte[] R;
        public readonly byte[] S;

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
            Unsafe.CopyBlock(ref flat[0], ref R[0], (uint) R.Length);
            // Array.Copy(R, 0, flat, 0, R.Length);
            Unsafe.CopyBlock(ref flat[R.Length], ref S[0], (uint) S.Length);
            // Array.Copy(S, 0, flat, R.Length, S.Length);
            flat[size - 1] = V;
            return flat;
        }
    }
}