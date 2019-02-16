namespace VeChainCore.Utils
{
    public static class CheckIfValid
    {
        public static bool Address(string address)
        {
            return address.Length == 42 && address.StartsWith("0x") && OnlyHexInString(address);
        }


        public static bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
        }
    }
}
