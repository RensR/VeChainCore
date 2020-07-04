using System;
using System.Runtime.Serialization;
using VeChainCore.Models.Core;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class VthoTransferClause : Clause
    {
        // implementation handles serialization

        public const string TransferMethodId = "0xa9059cbb";
        public const string ContractAddress = VTHO.ContractAddress;

        [DataMember, Obsolete("This is always the contract address.")]
        public string to => ContractAddress;

        [DataMember, Obsolete("The VET amount is zero as this is a contract operation.")]
        public decimal value => 0m;

        [DataMember]
        public string data { get; set; } = InitialData;

        [IgnoreDataMember]
        public string To
        {
            get => GetToField();
            set => SetToField(value);
        }

        [IgnoreDataMember]
        public decimal Value
        {
            get => GetValueField();
            set => SetValueField(value);
        }
    }
}