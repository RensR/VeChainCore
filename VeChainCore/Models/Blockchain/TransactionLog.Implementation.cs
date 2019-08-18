using System.Collections.Generic;

namespace VeChainCore.Models.Blockchain
{
    public partial class TransactionLog
    {
        public TransactionLog(
            string id,
            string origin,
            ulong size,
            TxMeta meta,
            Network chainTag,
            ulong blockRef,
            uint expiration,
            IEnumerable<Clause> clauses,
            ulong nonce,
            byte gasPriceCoef,
            ulong gas,
            string dependsOn
        ) : base(chainTag, blockRef, expiration, clauses, nonce, gasPriceCoef, gas, dependsOn)
        {
            this.id = id;
            this.origin = origin;
            this.size = size;
            this.meta = meta;
        }

        public TransactionLog(
            string id,
            string origin,
            ulong size,
            TxMeta meta,
            Network chainTag,
            ulong blockRef,
            uint expiration,
            List<Clause> clauses,
            ulong nonce,
            byte gasPriceCoef,
            ulong gas,
            string dependsOn
        ) : base(chainTag, blockRef, expiration, clauses, nonce, gasPriceCoef, gas, dependsOn)
        {
            this.id = id;
            this.origin = origin;
            this.size = size;
            this.meta = meta;
        }
    }
}