using System.Linq;
using System.Runtime.CompilerServices;

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
        public static byte[] Concat(this byte[] first, byte[] second)
        {
            var rv = new byte[first.Length + second.Length];
            if (first.Length > 0)
                Unsafe.CopyBlock(ref rv[0], ref first[0], (uint) first.Length);
            // Array.Copy(first, 0, rv, 0, first.Length);
            if (second.Length > 0)
                Unsafe.CopyBlock(ref rv[first.Length], ref second[0], (uint) second.Length);
            // Array.Copy(second, 0, rv, first.Length, second.Length);
            return rv;
        }

        /// <summary>
        /// Flattens the structure of an array of T arrays. The resulting T[] will contains all
        /// information found in all the arrays.
        /// </summary>
        /// <param name="arrays">An array of arrays of T</param>
        /// <returns>A combinated, flattened array of T</returns>
        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                if (array.Length > 0)
                    Unsafe.CopyBlock(ref rv[offset], ref array[0], (uint) array.Length);
                // Array.Copy(array, 0, rv, offset, array.Length);
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
            return array.TrimLeading(0);
        }

        /// <summary>
        /// Trims the leading instances of T b from an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array with potential leading instances of b</param>
        /// <param name="b">The object that will be trimmed</param>
        /// <returns>An array without leading instances of b</returns>
        public static byte[] TrimLeading(this byte[] array, byte b)
        {
            return array.SkipWhile(leading => leading.Equals(b)).ToArray();
        }
    }
}