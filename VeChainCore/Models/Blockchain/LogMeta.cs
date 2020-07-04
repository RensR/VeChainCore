using System;
using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class LogMeta : TxMeta, IEquatable<LogMeta>
    {
    }
}