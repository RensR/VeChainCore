using System;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Core;
using VeChainCore.Utils.Rlp;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;

namespace VeChainCore.Models.Extensions
{
    public static class RawTransactionExtension
    {
        public static RawTransaction CalculateTxId(this RawTransaction rawTransaction, Address signer)
        {
            if (rawTransaction.signature == null)
                throw new ArgumentException("transaction is not singed");

            var signingHash = Hash.HashBlake2B(rawTransaction.Encode());

            byte[] concatenatedBytes = new byte[52];
            Buffer.BlockCopy(signingHash, 0, concatenatedBytes, 0, signingHash.Length);
            Buffer.BlockCopy(signer.HexString.HexStringToByteArray(), 0, concatenatedBytes, signingHash.Length, signer.HexString.HexStringToByteArray().Length);
            byte[] txIdBytes = Hash.HashBlake2B(concatenatedBytes);
            rawTransaction.id = txIdBytes.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);
            return rawTransaction;
        }


        public static RawTransaction Sign(this RawTransaction transaction, ECKeyPair key)
        {
            var rlp = RlpEncoder.Encode(new RlpTransaction(transaction).AsRlpValues());

            SignatureData signatureData = ECDSASign.SignMessage(rlp, key, true);
            transaction.signature = signatureData.ToByteArray();

            return transaction;
        }

        public static TransferResult Transfer(this RawTransaction rawTransaction)
        {
            if(rawTransaction?.signature == null )
                throw new ArgumentException("Unsigned transaction can not be sent");

            var bytes = rawTransaction.Encode();

            var transactionString = bytes.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);

            return new TransferResult();
        }

        public static byte[] Encode(this RawTransaction rawTransaction)
        {
            return RlpEncoder.Encode(new RlpTransaction(rawTransaction).AsRlpValues());
        }
    }
}
