using System;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Models.Core
{
    public class RawTransaction
    {
        public string id { get; set; }
        public byte chainTag { get; set; }
        public string blockRef { get; set; }
        public uint expiration { get; set; }
        public RawClause[] clauses { get; set; }
        public byte gasPriceCoef { get; set; }
        public ulong gas { get; set; }
        public string origin { get; set; }
        public string nonce { get; set; }
        public string dependsOn { get; set; }
        public ulong size { get; set; }
        public TxMeta meta { get; set; }

        public string signature { get; set; }

        public static RawTransaction CreateUnsigned(
            byte chainTag,
            string blockRef,
            RawClause[] clauses,
            uint expiration = 720,
            byte gasPriceCoef = 100,
            ulong gas = 21000,
            string dependsOn = "")
        {

            if (clauses == null || clauses.Length < 1)
                throw new ArgumentException("No clauses found");

            return new RawTransaction
            {
                chainTag = chainTag,
                blockRef = blockRef,
                expiration = expiration,
                clauses = clauses,
                gasPriceCoef = gasPriceCoef,
                gas = gas,
                dependsOn = dependsOn
            };
        }
    }
}
