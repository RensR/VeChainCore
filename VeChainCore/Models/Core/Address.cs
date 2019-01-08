using System;
using VeChainCore.Utils;

namespace VeChainCore.Models.Core
{
    public class Address
    {
        public string HexString { get; set; }

        public Address(string address)
        {
            if(!IsValid(address))
                throw new ArgumentException("The string is not a hex string");

            HexString = address;
        }

        public static bool IsValid(string address)
        {
            if (!address.StartsWith(" 0x"))
                return false;
            if (!Hex.OnlyHexInString(address.Substring(2)))
                return false;
            return address.Length == 42;
        }
    }
}
