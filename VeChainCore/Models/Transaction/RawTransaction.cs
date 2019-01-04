using System;
using Nethereum.RLP;
using VeChainCore.Models.Extensions;
using VeChainCore.Models.Keys;

namespace VeChainCore.Models.Transaction
{
    public class RawTransaction
    {
        public byte[] chainTag { get; set; }
        public byte[] blockRef { get; set; }
        public byte[] expiration { get; set; }
        public byte[][][] clauses { get; set; }
        public byte[] gasPriceCoef { get; set; }
        public byte[] gas { get; set; }
        public byte[] dependsOn { get; set; }
        public byte[] nonce { get; set; }
        public byte[] reserved { get; set; }
        public byte[] signature { get; set; }


        public RawTransaction(Transaction transaction)
        {
            if (transaction.chainTag == 0)
                throw new ArgumentException("ChainTag is 0");
            chainTag = ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(transaction.chainTag);

            blockRef = transaction.blockRef.ToBytesForRLPEncoding() ?? throw new ArgumentException("BlockRef is null");

            if (transaction.expiration == 0)
                throw new ArgumentException("Expiration is 0");
            expiration = ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(transaction.expiration);

            clauses = transaction.clauses.GetRawClauses();

            gasPriceCoef = transaction.gasPriceCoef == 0 ? 
                RLP.EMPTY_BYTE_ARRAY : 
                ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(transaction.gasPriceCoef);

            if (transaction.gas == 0)
                throw new ArgumentException("Gas is 0");
            gas = ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(transaction.gas);

            dependsOn = transaction.dependsOn == null ? RLP.EMPTY_BYTE_ARRAY : transaction.dependsOn.ToBytesForRLPEncoding();

            nonce = transaction.nonce.ToBytesForRLPEncoding() ?? throw new ArgumentException("Nonce is null");

            reserved = null;
        }

        public static RawTransaction SignRawTransaction(RawTransaction rawTransaction, ECKeyPair keyPair)
        {
            return rawTransaction.Sign(keyPair);
        }
    }
}
