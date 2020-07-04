using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    public partial class TransactionLog : Transaction
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public override string id { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string origin { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ulong size { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TxMeta meta { get; set; }
    }
}