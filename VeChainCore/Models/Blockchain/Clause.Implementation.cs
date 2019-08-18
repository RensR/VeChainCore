using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    public abstract partial class Clause
    {
        [IgnoreDataMember]
        public abstract byte[] RLPData { get; }

        protected abstract string GetTo();
        protected abstract decimal GetValue();
        protected abstract string GetData();
    }
}