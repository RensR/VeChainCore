using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;
using VeChainCore.Client;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public partial class LogMeta : TxMeta, IEquatable<LogMeta>
    {
    }
}