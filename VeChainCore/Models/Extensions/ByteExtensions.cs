using System.Linq;

namespace VeChainCore.Models.Extensions
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Concatenates two T arrays. The content of the first argument will come before the
        /// content of the second argument
        /// </summary>
        /// <param name="first">The first T[]</param>
        /// <param name="second">The second T[]</param>
        /// <returns></returns>
        public static T[] Concat<T>(this T[] first, T[] second)
        {
            var rv = new T[first.Length + second.Length];
            System.Buffer.BlockCopy(first, 0, rv, 0, first.Length);
            System.Buffer.BlockCopy(second, 0, rv, first.Length, second.Length);
            return rv;
        }

        /// <summary>
        /// Flattens the structure of an array of T arrays. The resulting T[] will contains all
        /// information found in all the arrays.
        /// </summary>
        /// <param name="arrays">An array of arrays of T</param>
        /// <returns>A combinated, flattened array of T</returns>
        public static T[] Combine<T>(params T[][] arrays)
        {
            T[] rv = new T[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (T[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        /// <summary>
        /// Trims the leading zero's of a byte[]
        /// </summary>
        /// <param name="array">The array with potential leading 0's</param>
        /// <returns>An array without leading 0's</returns>
        public static byte[] TrimLeadingZeroBytes(this byte[] array)
        {
            return array.TrimLeading<byte>(0);
        }

        /// <summary>
        /// Trims the leading instances of T b from an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array with potential leading instances of b</param>
        /// <param name="b">The object that will be trimmed</param>
        /// <returns>An array without leading instances of b</returns>
        public static T[] TrimLeading<T>(this T[] array, T b)
        {
            return array.SkipWhile(leading => leading.Equals(b)).ToArray();
        }
    }
}
