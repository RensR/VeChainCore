using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Math;
using Utf8Json;
using VeChainCore.Models.Extensions;

namespace VeChainCore.Utils.Json
{
    public sealed class VeChainHexFormatter
        :
            IJsonFormatter<decimal>,
            IJsonFormatter<BigInteger>,
            IJsonFormatter<ulong>,
            IJsonFormatter<ulong?>,
            IJsonFormatter<byte[]>
    {
        public static readonly IJsonFormatter Default = new VeChainHexFormatter();
        public static readonly IJsonFormatter TrimLeadingZeros = new VeChainHexFormatter(true);
        private readonly bool _trimLeadingZeros;

        public VeChainHexFormatter(bool trimLeadingZeros = false)
        {
            _trimLeadingZeros = trimLeadingZeros;
        }

        public void Serialize(
            ref JsonWriter writer,
            Decimal value,
            IJsonFormatterResolver formatterResolver)
        {
            Serialize(ref writer, value.ToBigInteger(), formatterResolver);
        }

        public void Serialize(ref JsonWriter writer, BigInteger value, IJsonFormatterResolver formatterResolver)
        {
            Serialize(ref writer, value.ToByteArrayUnsigned(), formatterResolver);
        }

        public void Serialize(ref JsonWriter writer, ulong value, IJsonFormatterResolver formatterResolver)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            bytes.TrimLeading();
            Serialize(ref writer, bytes, formatterResolver);
        }

        private static void WriteHexCharUnsafe(ref JsonWriter writer, int nib)
        {
            if (nib <= 9)
                writer.WriteRawUnsafe((byte) ('0' + nib));
            else
                writer.WriteRawUnsafe((byte) ('a' + (nib - 0xa)));
        }

        public void Serialize(ref JsonWriter writer, byte[] value, IJsonFormatterResolver formatterResolver)
        {
            int length = value.Length;
            bool noBytes = length == 0;
            writer.EnsureCapacity((noBytes ? 1 : length) * 2 + 4);
            writer.WriteRawUnsafe((byte) '"');
            writer.WriteRawUnsafe((byte) '0');
            writer.WriteRawUnsafe((byte) 'x');
            if (noBytes)
            {
                writer.WriteRawUnsafe((byte) '0');
            }
            else
            {
                bool writtenNonZero = false;
                for (int i = 0; i < length; i++)
                {
                    byte aByte = value[i];

                    int hiNib = aByte >> 4;
                    int loNib = aByte & 0xF;

                    if (_trimLeadingZeros && !writtenNonZero)
                    {
                        if (hiNib == 0)
                        {
                            if (loNib == 0)
                                continue;

                            WriteHexCharUnsafe(ref writer, loNib);
                            writtenNonZero = true;
                            continue;
                        }
                        writtenNonZero = true;
                    }

                    WriteHexCharUnsafe(ref writer, hiNib);
                    WriteHexCharUnsafe(ref writer, loNib);
                }
            }

            writer.WriteRawUnsafe((byte) '"');
        }


        public byte[] DeserializeBytes(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var currentJsonToken = reader.GetCurrentJsonToken();
            switch (currentJsonToken)
            {
                case JsonToken.String:
                {
                    string s = reader.ReadString();
                    return s.HexToByteArray();
                }

                case JsonToken.Number:
                {
                    var n = reader.ReadUInt64();
                    return BitConverter.GetBytes(n).Reverse().ToArray();
                }

                default:
                    throw new InvalidOperationException("Invalid Json Token for VeChainHexFormatter:" + currentJsonToken);
            }
        }

        public BigInteger DeserializeBigInteger(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => new BigInteger(1, DeserializeBytes(ref reader, formatterResolver));

        public ulong DeserializeUInt64(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var bytes = DeserializeBytes(ref reader, formatterResolver);
            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse().ToArray();
            bytes = bytes.PadLeading(8);
            return Unsafe.ReadUnaligned<ulong>(ref bytes[0]);
        }

        public Decimal DeserializeDecimal(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeBigInteger(ref reader, formatterResolver).ToDecimal();

        void IJsonFormatter<ulong?>.Serialize(ref JsonWriter writer, ulong? value, IJsonFormatterResolver formatterResolver)
        {
            if ( value != null )
                Serialize(ref writer, value.Value, formatterResolver);
            else
                throw new NotImplementedException();
        }

        ulong? IJsonFormatter<ulong?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) =>
            DeserializeUInt64(ref reader, formatterResolver);

        byte[] IJsonFormatter<byte[]>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeBytes(ref reader, formatterResolver);

        ulong IJsonFormatter<ulong>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeUInt64(ref reader, formatterResolver);

        BigInteger IJsonFormatter<BigInteger>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeBigInteger(ref reader, formatterResolver);

        decimal IJsonFormatter<decimal>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => DeserializeDecimal(ref reader, formatterResolver);
    }
}