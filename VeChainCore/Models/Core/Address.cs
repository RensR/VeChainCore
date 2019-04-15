using System;
using System.Linq;
using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;

namespace VeChainCore.Models.Core
{
    public class Address : IRLPElement
    {
        public string HexString { get; set; }

        private byte[] Bytes
            => HexString.HexToByteArray().TrimLeading();

        public Address(string address)
        {
            if (!IsValid(address))
                throw new ArgumentException("The string is not a hex string");

            HexString = address;
        }

        public static bool IsValid(string address)
        {
            if (!address.StartsWith("0x"))
                return false;

            if (address.Skip(2)
                .Any(c => (c < '0' || c > '9')
                          && (c < 'A' || c > 'F')
                          && (c < 'a' || c > 'f')))
                return false;

            return address.Length == 42;
        }

        [IgnoreDataMember]
        public byte[] RLPData
            => RLP.EncodeElement(Bytes);
    }
}