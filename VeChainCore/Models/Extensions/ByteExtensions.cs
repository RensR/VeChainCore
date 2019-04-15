using System;
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
            if (second.Length > 0)
                Unsafe.CopyBlock(ref rv[first.Length], ref second[0], (uint) second.Length);
            return rv;
        }

        /// <summary>
        /// Flattens the structure of an array of byte arrays.
        /// The resulting byte array will contain all
        /// information found in all the arrays in the given
        /// order.
        /// </summary>
        /// <param name="arrays">An array of arrays of bytes.</param>
        /// <returns>A flattened array of bytes.</returns>
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
        /// Flattens the structure of an array of byte arrays.
        /// The resulting byte array will contain all
        /// information found in all the arrays in the given
        /// order.
        /// </summary>
        /// <param name="arrays">An array of arrays of bytes.</param>
        /// <returns>A flattened array of bytes.</returns>
        public static byte[] Flatten(this byte[][] arrays)
            => Combine(arrays);

        /// <summary>
        /// Pads the array with the leading byte to a desired length.
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <param name="desiredLength">The desired length of the array.</param>
        /// <param name="value">The value of the padding byte.</param>
        /// <returns>An array with leading bytes.</returns>
        public static byte[] PadLeading(this byte[] buffer, int desiredLength, byte value = 0)
        {
            var bytes = new byte[desiredLength];

            if (desiredLength == buffer.Length)
                return buffer;

            if (desiredLength < buffer.Length)
                throw new ArgumentException("Array longer than desired padded length.", nameof(desiredLength));

            int paddingLength = desiredLength - buffer.Length;

            if (value != 0 && paddingLength > 0)
                Unsafe.InitBlock(ref bytes[0], value, (uint) paddingLength);

            if (buffer.Length > 0)
                Unsafe.CopyBlock(ref bytes[paddingLength], ref buffer[0], (uint) buffer.Length);

            return bytes;
        }

        /// <summary>
        /// Trims the leading bytes from a byte array.
        /// </summary>
        /// <param name="buffer">The array with leading instances of value.</param>
        /// <param name="value">The value that will be trimmed.</param>
        /// <returns>An array without leading bytes.</returns>
        public static byte[] TrimLeading(this byte[] buffer, byte value = 0)
        {
            var leadingCount = 0;
            while (leadingCount < buffer.Length)
            {
                if (buffer[leadingCount] != value)
                    break;
                ++leadingCount;
            }

            uint bufferLength = (uint) (buffer.Length - leadingCount);

            return buffer.LastBytes(bufferLength);
        }

        public static byte[] FirstBytes(this byte[] buffer, uint length)
        {
            if (buffer.Length < length)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than length of buffer.");

            if (buffer.Length == 0)
                return Array.Empty<byte>();

            if (buffer.Length == length)
                return buffer;

            var bytes = new byte[length];

            Unsafe.CopyBlock(ref bytes[0], ref buffer[0], length);

            return bytes;
        }

        public static byte[] LastBytes(this byte[] buffer, uint length)
        {
            if (buffer.Length < length)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than length of buffer.");

            if (buffer.Length == 0)
                return Array.Empty<byte>();

            if (buffer.Length == length)
                return buffer;

            var bytes = new byte[length];

            Unsafe.CopyBlock(ref bytes[0], ref buffer[buffer.Length - length], length);

            return bytes;
        }
    }
}