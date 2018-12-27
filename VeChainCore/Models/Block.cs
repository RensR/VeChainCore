using System;
using System.Collections.Generic;
using System.Linq;

namespace VeChainCore.Models
{
    public class Block : IEquatable<Block>
    {
        public uint number { get; set; }
        public string id { get; set; }
        public uint size { get; set; }
        public string parentID { get; set; }
        public uint timestamp { get; set; }
        public uint gasLimit { get; set; }
        public string beneficiary { get; set; }
        public uint gasUsed { get; set; }
        public uint totalScore { get; set; }
        public string txsRoot { get; set; }
        public string stateRoot { get; set; }
        public string receiptsRoot { get; set; }
        public string signer { get; set; }
        public bool isTrunk { get; set; }
        public Transaction[] transactions { get; set; }

        public override string ToString()
        {
            return $"Block: {number}";
        }

        public override bool Equals(object other)
        {
            if (!(other is Block))
                return false;
            return Equals((Block) other);
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
