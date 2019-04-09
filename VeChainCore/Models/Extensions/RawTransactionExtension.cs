using System;
using System.Threading.Tasks;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Core;
using VeChainCore.Utils.Rlp;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;

namespace VeChainCore.Models.Extensions
{
    public static class RawTransactionExtension
    {
        /// <summary>
        /// Calculates the transaction id of a signed transaction. Will throw an exception when the transaction
        /// is not signed. The id is not needed to submit a transaction but can be useful when transactions
        /// depend on each other.
        /// </summary>
        /// <param name="rawTransaction">The RawTransaction for which the id needs to be calculated.</param>
        /// <param name="signer">The address of the signer.</param>
        /// <returns>The RawTransaction with the calculated id.</returns>
        public static RawTransaction CalculateTxId(this RawTransaction rawTransaction, Address signer)
        {
            if (rawTransaction.signature == null)
                throw new ArgumentException("transaction is not singed");

            // Hash the RLP encoded transaction
            var signingHash = Hash.HashBlake2B(rawTransaction.Encode());
            var signingAddress = signer.HexString.HexStringToByteArray();

            byte[] concatenatedBytes = new byte[52];
            Buffer.BlockCopy(signingHash,    0, concatenatedBytes, 0, signingHash.Length);
            Buffer.BlockCopy(signingAddress, 0, concatenatedBytes, signingHash.Length, signingAddress.Length);

            // Hash the bytes from the signed transaction and the signer address
            byte[] txIdBytes = Hash.HashBlake2B(concatenatedBytes);
            rawTransaction.id = txIdBytes.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);
            return rawTransaction;
        }

        /// <summary>
        /// Sign the transaction by RLP encoding it and calculating the signature of the resulting byte[].
        /// </summary>
        /// <param name="transaction">The transaction that will be signed.</param>
        /// <param name="key">The key with which the transaction will be signed.</param>
        /// <returns>The signed transaction</returns>
        public static RawTransaction Sign(this RawTransaction transaction, ECKeyPair key)
        {
            var rlp = RlpEncoder.Encode(new RlpTransaction(transaction).AsRlpValues());

            SignatureData signatureData = ECDSASign.SignMessage(rlp, key, true);
            transaction.signature = signatureData.ToByteArray();

            return transaction;
        }

        /// <summary>
        /// Transfer the transaction to the blockchain to be included in the next block.
        /// </summary>
        /// <param name="rawTransaction">The transaction to be included in the blockchain</param>
        /// <param name="client">The client that determines the interaction with the blockchain</param>
        /// <returns>The result of the transfer</returns>
        public static async Task<TransferResult> Transfer(this RawTransaction rawTransaction, VeChainClient client)
        {
            if(rawTransaction?.signature == null )
                throw new ArgumentException("Unsigned transaction can not be sent");

            var bytes = rawTransaction.Encode();

            var transactionString = bytes.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);

            return await client.SendTransaction(bytes);
        }

        /// <summary>
        /// RLP encode the transaction and turn it into a byte[]
        /// </summary>
        /// <param name="rawTransaction">The transaction that is to be encoded</param>
        /// <returns>The encoded transaction</returns>
        public static byte[] Encode(this RawTransaction rawTransaction)
        {
            return RlpEncoder.Encode(new RlpTransaction(rawTransaction).AsRlpValues());
        }
    }
}
