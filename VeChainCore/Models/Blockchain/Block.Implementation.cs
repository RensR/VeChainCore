using System;
using System.Collections.Generic;
using System.Linq;
using Utf8Json;
using VeChainCore.Client;

namespace VeChainCore.Models.Blockchain
{
    public partial class Block
    {
        public override string ToString()
        {
            return number != 0
                ? $"{{\"number\":{number}}}"
                : JsonSerializer.ToJsonString(this, VeChainClient.JsonFormatterResolver);
        }

        public override bool Equals(object other)
        {
            return other is Block block && Equals(block);
        }

        public bool Equals(Block other)
        {
            return other != null
                   && number == other.number
                   && id == other.id
                   && size == other.size
                   && parentID == other.parentID
                   && timestamp == other.timestamp
                   && gasLimit == other.gasLimit
                   && beneficiary == other.beneficiary
                   && gasUsed == other.gasUsed
                   && totalScore == other.totalScore
                   && txsRoot == other.txsRoot
                   && stateRoot == other.stateRoot
                   && receiptsRoot == other.receiptsRoot
                   && signer == other.signer
                   && isTrunk == other.isTrunk
                   && transactions.SequenceEqual(other.transactions);
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