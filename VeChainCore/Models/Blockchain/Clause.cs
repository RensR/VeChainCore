using System.Runtime.Serialization;
using Nethereum.RLP;

namespace VeChainCore.Models.Blockchain
{
    public abstract class Clause : IRLPElement
    {
        public abstract string to { get; }
        public abstract decimal value { get; }
        public abstract string data { get; }

        [IgnoreDataMember]
        public abstract byte[] RLPData { get; }
    }
}