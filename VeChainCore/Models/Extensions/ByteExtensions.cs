﻿using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace VeChainCore.Models.Extensions
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Pads the array with the leading byte to a desired length.
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <param name="desiredLength">The desired length of the array.</param>
        /// <param name="value">The value of the padding byte.</param>
        /// <param name="allowLonger">If true, over-length input buffers are returned instead of being in error.</param>
        /// <returns>An array with leading bytes.</returns>
        public static byte[] PadLeading(this byte[] buffer, int desiredLength, byte value = 0, bool allowLonger = true)
        {
            if (desiredLength == buffer.Length)
                return buffer;

            if (desiredLength < buffer.Length)
            {
                if (allowLonger)
                    return buffer;

                throw new ArgumentException("Array longer than desired padded length.", nameof(desiredLength));
            }

            var bytes = new byte[desiredLength];

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

            var bufferLength = (uint) (buffer.Length - leadingCount);

            return bufferLength == 0
                ? Array.Empty<byte>()
                : buffer.LastBytes(bufferLength);
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

        public static byte[] ToBytes(this string hex) 
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            var arr = new byte[hex.Length >> 1];

            for (var i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex) 
        {
            var val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        
        public static void Append(this MemoryStream stream, byte value)
        {
            stream.Append(new[] { value });
        }

        public static void Append(this MemoryStream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }
    }
}