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
    public partial class VetClause
    {
        public override string to { get; }
        public override decimal value { get; }
        public override string data { get; }
    }
}