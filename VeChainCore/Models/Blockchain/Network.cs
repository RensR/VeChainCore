using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    /// <summary>
    /// The chaintags of the three known VeChain networks
    /// </summary>
    [DataContract]
    public enum Network : byte
    {
        [DataMember]
        Invalid = 0,

        [DataMember]
        Test = 39,

        [DataMember]
        Main = 74,

        [DataMember]
        Dev = 164
    }
}