using System.Linq;

namespace VeChainCore.Models.Extensions
{
    public static class ByteExtensions
    {
        public static byte[] Concat(this byte[] first, byte[] second)
        {
            var rv = new byte[first.Length + second.Length];
            System.Buffer.BlockCopy(first, 0, rv, 0, first.Length);
            System.Buffer.BlockCopy(second, 0, rv, first.Length, second.Length);
            return rv;
        }


        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }


        public static byte[] TrimLeadingZeroBytes(this byte[] array)
        {
            return array.TrimLeadingByte((byte) 0);
        }

        public static byte[] TrimLeadingByte(this byte[] array, byte b)
        {
            return array.SkipWhile(leading => leading == b).ToArray();
        }
    }
}
