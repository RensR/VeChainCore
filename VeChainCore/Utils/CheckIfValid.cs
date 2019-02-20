namespace VeChainCore.Utils
{
    public static class CheckIfValid
    {
        /// <summary>
        /// A valid VeChain address should be 42 characters long and start with 0x and contain only
        /// hex characters after the 0x.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool Address(string address)
        {
            return address.Length == 42 && address.StartsWith("0x") && address.IsHexString();
        }
    }
}
