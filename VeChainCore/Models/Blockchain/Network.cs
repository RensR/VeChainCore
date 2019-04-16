namespace VeChainCore.Models.Blockchain
{
    /// <summary>
    /// The chaintags of the three known VeChain networks
    /// </summary>
    public enum Network : byte
    {
        Invalid = 0,
        Test = 39,
        Main = 74,
        Dev = 164
    }
}