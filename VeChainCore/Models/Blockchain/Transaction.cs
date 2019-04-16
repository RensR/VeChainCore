using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Math;
using Utf8Json;
using VeChainCore.Client;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils.Json;

namespace VeChainCore.Models.Blockchain
{
    public partial class Transaction
    {

        public Network chainTag { get; set; }
        public string blockRef { get; set; }
        public uint expiration { get; set; }
        public Clause[] clauses { get; set; }
        public byte gasPriceCoef { get; set; }
        
        [JsonFormatter(typeof(VeChainHexFormatter))]
        public ulong gas { get; set; }
        public string dependsOn { get; set; }
        
        [JsonFormatter(typeof(VeChainHexFormatter))]
        public ulong nonce { get; set; }

        private static readonly byte[] Reserved = {0xc0};

        public string signature { get; set; }

        public string id { get; set; }
        public string origin { get; set; }
        public ulong size { get; set; }
        public TxMeta meta { get; set; }
    }
}