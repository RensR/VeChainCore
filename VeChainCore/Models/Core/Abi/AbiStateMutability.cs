using System.ComponentModel;

namespace VeChainCore.Models.Core.Abi
{
    public enum AbiStateMutability
    {
        [Description("pure")]
        Pure,
        [Description("view")]
        View,
        [Description("constant")]
        Constant,
        [Description("payable")]
        Payable,
        [Description("nonpayable")]
        Nonpayable
    }
}