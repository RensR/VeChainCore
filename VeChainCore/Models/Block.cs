using System;
using System.Collections.Generic;
using System.Linq;

namespace VeChainCore
{
    public class Block : IEquatable<Block>
    {
        public UInt32 number;
        public string id;
        public UInt32 size;
        public string parentID;
        public UInt64 timestamp;
        public UInt64 gasLimit;
        public string beneficiary;
        public UInt64 gasUsed;
        public UInt64 totalScore;
        public string txsRoot;
        public string stateRoot;
        public string receiptsRoot;
        public string signer;
        public bool isTrunk;
        public Transaction[] transactions;

        public override string ToString()
        {
            string block = $"Block: {number}";
            return base.ToString();
        }

        public override bool Equals(object other)
        {
            if (!(other is Block toCompareWith))
                return false;
            return Equals(other as Block);
        }

        public bool Equals(Block other)
        {
            var equal = other != null &&
                   number == other.number &&
                   id == other.id &&
                   size == other.size &&
                   parentID == other.parentID &&
                   timestamp == other.timestamp &&
                   gasLimit == other.gasLimit &&
                   beneficiary == other.beneficiary &&
                   gasUsed == other.gasUsed &&
                   totalScore == other.totalScore &&
                   txsRoot == other.txsRoot &&
                   stateRoot == other.stateRoot &&
                   receiptsRoot == other.receiptsRoot &&
                   signer == other.signer &&
                   isTrunk == other.isTrunk;

            var transactionEqual = transactions.SequenceEqual(other.transactions);
            return equal && transactionEqual;
        }

        public static bool operator ==(Block clause1, Block clause2)
        {
            return EqualityComparer<Block>.Default.Equals(clause1, clause2);
        }

        public static bool operator !=(Block clause1, Block clause2)
        {
            return !(clause1 == clause2);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(number);
            hash.Add(id);
            hash.Add(size);
            hash.Add(parentID);
            hash.Add(timestamp);
            hash.Add(gasLimit);
            hash.Add(beneficiary);
            hash.Add(gasUsed);
            hash.Add(totalScore);
            hash.Add(txsRoot);
            hash.Add(stateRoot);
            hash.Add(receiptsRoot);
            hash.Add(signer);
            hash.Add(isTrunk);
            hash.Add(transactions);
            return hash.ToHashCode();
        }
    }
}
