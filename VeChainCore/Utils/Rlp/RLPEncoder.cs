using System;
using System.Linq;
using VeChainCore.Models.Extensions;

namespace VeChainCore.Utils.Rlp
{
    public class RlpEncoder
    {
        /**
        * [0x80]
        * If a string is 0-55 bytes long, the RLP encoding consists of a single
        * byte with value 0x80 plus the length of the string followed by the
        * string. The range of the first byte is thus [0x80, 0xb7].
        */
        public static int OFFSET_SHORT_STRING = 0x80;

        /**
         * [0xb7]
         * If a string is more than 55 bytes long, the RLP encoding consists of a
         * single byte with value 0xb7 plus the length of the length of the string
         * in binary form, followed by the length of the string, followed by the
         * string. For example, a length-1024 string would be encoded as
         * \xb9\x04\x00 followed by the string. The range of the first byte is thus
         * [0xb8, 0xbf].
         */
        public static int OFFSET_LONG_STRING = 0xb7;

        /**
         * [0xc0]
         * If the total payload of a list (i.e. the combined length of all its
         * items) is 0-55 bytes long, the RLP encoding consists of a single byte
         * with value 0xc0 plus the length of the list followed by the concatenation
         * of the RLP encodings of the items. The range of the first byte is thus
         * [0xc0, 0xf7].
         */
        public static int OFFSET_SHORT_LIST = 0xc0;

        /**
         * [0xf7]
         * If the total payload of a list is more than 55 bytes long, the RLP
         * encoding consists of a single byte with value 0xf7 plus the length of the
         * length of the list in binary form, followed by the length of the list,
         * followed by the concatenation of the RLP encodings of the items. The
         * range of the first byte is thus [0xf8, 0xff].
         */
        public static int OFFSET_LONG_LIST = 0xf7;


        public static byte[] Encode(IRlpType value)
        {
            if (value is RlpString item)
                return EncodeString(item);
            return EncodeList(value as RlpList);
        }

        private static byte[] Encode(byte[] bytesValue, int offset)
        {
            if (bytesValue.Length == 1
                && offset == OFFSET_SHORT_STRING
                && bytesValue[0] >= (byte) 0x00
                && bytesValue[0] <= (byte) 0x7f)
            {
                return bytesValue;
            }
            else if (bytesValue.Length < 55)
            {
                byte[] result = new byte[bytesValue.Length + 1];
                result[0] = (byte) (offset + bytesValue.Length);
                Buffer.BlockCopy(bytesValue, 0, result, 1, bytesValue.Length);
                return result;
            }
            else
            {
                byte[] encodedStringLength = ToMinimalByteArray(bytesValue.Length);
                byte[] result = new byte[bytesValue.Length + encodedStringLength.Length + 1];

                result[0] = (byte) ((offset + 0x37) + encodedStringLength.Length);
                Buffer.BlockCopy(encodedStringLength, 0, result, 1, encodedStringLength.Length);
                Buffer.BlockCopy(
                    bytesValue, 0, result, encodedStringLength.Length + 1, bytesValue.Length);
                return result;
            }
        }

        static byte[] EncodeString(RlpString value)
        {
            return Encode(value.GetBytes(), OFFSET_SHORT_STRING);
        }

        public static byte[] ToMinimalByteArray(int value)
        {
            return toByteArray(value).SkipWhile(element => element == 0).ToArray();
        }

        private static byte[] toByteArray(int value)
        {
            return new[]
            {
                (byte) ((value >> 24) & 0xff),
                (byte) ((value >> 16) & 0xff),
                (byte) ((value >> 8) & 0xff),
                (byte) (value & 0xff)
            };
        }

        private static byte[] EncodeList(RlpList value)
        {
            var values = value.GetValues();

            if (values.Count < 1)
            {
                return Encode(new byte[] { }, OFFSET_SHORT_LIST);
            }

            byte[] result = new byte[0];
            foreach (IRlpType entry in values)
            {
                result = result.Concat(Encode(entry));
            }
  
            return Encode(result, OFFSET_SHORT_LIST);
        }
    }
}
