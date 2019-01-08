using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VeChainCore.Utils;
using VeChainCore.Utils.Rlp;

namespace VeChainCore.Models.Transaction
{
    public class RlpTransaction
    {
        public RlpString chainTag { get; set; }
        public RlpString blockRef { get; set; }
        public RlpString expiration { get; set; }
        public RlpList clauses { get; set; }
        public RlpString gasPriceCoef { get; set; }
        public RlpString gas { get; set; }
        public RlpString dependsOn { get; set; }
        public RlpString nonce { get; set; }
        public RlpList reserved { get; set; }


        public RlpTransaction(Transaction transaction)
        {
            if (transaction.chainTag == 0)
                throw new ArgumentException("ChainTag is 0");
            chainTag = RlpString.Create(transaction.chainTag);

            if (transaction.blockRef is null)
                throw new ArgumentException("BlockRef is 0");
            blockRef = RlpString.Create(transaction.blockRef.HexStringToByteArray());

            if (transaction.expiration == 0)
                throw new ArgumentException("Expiration is 0");
            expiration = RlpString.Create(transaction.expiration);

            clauses = new RlpList(transaction.clauses.Select(ToRlpList).ToList());

            gasPriceCoef = transaction.gasPriceCoef == 0
                ? RlpString.Create(RlpString.EMPTY)
                : RlpString.Create(transaction.gasPriceCoef);

            if (transaction.gas == 0)
                throw new ArgumentException("Gas is 0");
            gas = RlpString.Create(transaction.gas);

            dependsOn = transaction.dependsOn == null
                ? RlpString.Create(RlpString.EMPTY)
                : RlpString.Create(transaction.dependsOn);

            if (transaction.nonce is null)
                throw new ArgumentException("Nonce is null");
            nonce = RlpString.Create(transaction.nonce);

            var emptyList = new List<RlpType>();
            reserved = new RlpList(emptyList);
        }

        public RlpType ToRlpList(Clause clause)
        {
            return new RlpList(new List<RlpType>
            {
                RlpString.Create(clause.to.HexStringToByteArray()),
                RlpString.Create(BigInteger.Parse(clause.value)),
                RlpString.Create(clause.data.HexStringToByteArray())
            });
        }

        public RlpList AsRLPValues()
        {
            return new RlpList(new List<RlpType>
            {
                chainTag,
                blockRef,
                expiration,
                clauses,
                gasPriceCoef,
                gas,
                dependsOn,
                nonce,
                reserved
            });
        }
    }
}
