using System;
using System.Collections.Generic;
using System.Linq;
using VeChainCore.Utils;
using VeChainCore.Utils.Rlp;

namespace VeChainCore.Models.Core
{
    public class RlpTransaction
    {
        public RlpString ChainTag { get; set; }
        public RlpString BlockRef { get; set; }
        public RlpString Expiration { get; set; }
        public RlpList Clauses { get; set; }
        public RlpString GasPriceCoef { get; set; }
        public RlpString Gas { get; set; }
        public RlpString DependsOn { get; set; }
        public RlpString Nonce { get; set; }
        public RlpList Reserved { get; set; }
        public RlpString Signature { get; set; }


        public RlpTransaction(RawTransaction transaction)
        {
            if (transaction.chainTag == 0)
                throw new ArgumentException("ChainTag is 0");
            ChainTag = RlpString.Create(transaction.chainTag);

            if (transaction.blockRef is null)
                throw new ArgumentException("BlockRef is 0");
            BlockRef = RlpString.Create(UInt64.Parse(transaction.blockRef, System.Globalization.NumberStyles.HexNumber));

            if (transaction.expiration == 0)
                throw new ArgumentException("Expiration is 0");
            Expiration = RlpString.Create(transaction.expiration);


            Clauses = new RlpList();
            Clauses.AddRange(transaction.clauses.Select(ToRlpList));

            GasPriceCoef = transaction.gasPriceCoef == 0
                ? RlpString.Create(RlpString.EMPTY)
                : RlpString.Create(transaction.gasPriceCoef);

            if (transaction.gas == 0)
                throw new ArgumentException("Gas is 0");
            Gas = RlpString.Create(transaction.gas);

            DependsOn = transaction.dependsOn == null
                ? RlpString.Create(RlpString.EMPTY)
                : RlpString.Create(transaction.dependsOn);

            if (transaction.nonce is null)
                throw new ArgumentException("Nonce is null");
            Nonce = RlpString.Create(UInt64.Parse(transaction.nonce));

            RlpList reservedList = null;
            Reserved = reservedList;

            if (transaction.signature != null)
                Signature = RlpString.Create(transaction.signature);
        }

        public IRlpType ToRlpList(RawClause clause)
        {
            var list = new RlpList();
            list.AddRange(new RlpList
            {
                RlpString.Create(clause.To.HexString.HexStringToByteArray()),
                RlpString.Create(clause.Value.AsBytes),
                RlpString.Create(clause.Data.HexStringToByteArray())
             });
            return list;
        }

        public RlpList AsRlpValues()
        {
            var rlpList = new RlpList();

            rlpList.AddRange(new List<IRlpType>
            {
                ChainTag,
                BlockRef,
                Expiration,
                Clauses,
                GasPriceCoef,
                Gas,
                DependsOn,
                Nonce,
                Reserved
            });

            if(Signature != null)
                rlpList.Add(Signature);

            return rlpList;
        }
    }
}
