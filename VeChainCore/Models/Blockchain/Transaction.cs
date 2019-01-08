using System;
using System.Collections.Generic;

namespace VeChainCore.Models.Blockchain
{
    public class Transaction : IEquatable<Transaction>
    {
        public string id { get; set; }
        public byte chainTag { get; set; }
        public string blockRef { get; set; }
        public uint expiration { get; set; }
        public Clause[] clauses { get; set; }
        public byte gasPriceCoef { get; set; }
        public ulong gas { get; set; }
        public string origin { get; set; }
        public string nonce { get; set; }
        public string dependsOn { get; set; }
        public ulong size { get; set; }
        public TxMeta meta { get; set; }

        public string signature { get; set; }

        public static Transaction CreateUnsigned(
            byte chainTag, 
            string blockRef,
            Clause[] clauses,
            uint expiration = 720, 
            byte gasPriceCoef = 100, 
            ulong gas = 21000,
            string dependsOn = "")
        {

            //byte chainTag = BlockchainClient.getChainTag();
            //byte[] blockRef = BlockchainClient.getBlockRef(Revision.BEST).toByteArray();
            //Amount amount = Amount.createFromToken(AbstractToken.VET);
            //amount.setDecimalAmount("1.12");
            //ToClause clause = TransactionClient.buildVETToClause
            //(Address.fromHexString("0xD3EF28DF6b553eD2fc47259E8134319cB1121A2A"), amount, ToData.ZERO);

            //RawTransaction rawTransaction = RawTransactionFactory.getInstance().createRawTransaction
            //(chainTag, blockRef, 720, 21000, (byte)0x0, CryptoUtils.generateTxNonce(), clause);

            if(clauses == null || clauses.Length < 1)
                throw  new ArgumentException("No clauses found");

            return new Transaction
            {
                chainTag = chainTag,
                blockRef =  blockRef,
                expiration = expiration,
                clauses = clauses,
                gasPriceCoef = gasPriceCoef,
                gas = gas,
                dependsOn =  dependsOn
            };
        }

        public override string ToString()
        {
            return $"Transaction {id}";
        }

        public bool Equals(Transaction other)
        {
            return other != null &&
                   id == other.id &&
                   chainTag == other.chainTag &&
                   blockRef == other.blockRef &&
                   expiration == other.expiration &&
                   EqualityComparer<Clause[]>.Default.Equals(clauses, other.clauses) &&
                   gasPriceCoef == other.gasPriceCoef &&
                   gas == other.gas &&
                   origin == other.origin &&
                   nonce == other.nonce &&
                   dependsOn == other.dependsOn &&
                   size == other.size &&
                   EqualityComparer<TxMeta>.Default.Equals(meta, other.meta);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(id);
            hash.Add(chainTag);
            hash.Add(blockRef);
            hash.Add(expiration);
            hash.Add(clauses);
            hash.Add(gasPriceCoef);
            hash.Add(gas);
            hash.Add(origin);
            hash.Add(nonce);
            hash.Add(dependsOn);
            hash.Add(size);
            hash.Add(meta);
            return hash.ToHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Transaction);
        }

        public static bool operator ==(Transaction clause1, Transaction clause2)
        {
            return EqualityComparer<Transaction>.Default.Equals(clause1, clause2);
        }

        public static bool operator !=(Transaction clause1, Transaction clause2)
        {
            return !(clause1 == clause2);
        }
    }
}
