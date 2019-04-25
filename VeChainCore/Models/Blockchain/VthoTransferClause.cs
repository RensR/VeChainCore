using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Org.BouncyCastle.Math;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;

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