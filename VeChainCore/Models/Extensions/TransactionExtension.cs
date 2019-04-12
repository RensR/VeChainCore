using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Core;
using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;
using Transaction = VeChainCore.Models.Blockchain.Transaction;

namespace VeChainCore.Models.Extensions
{
    public static class TransactionExtension
    {
        /// <summary>
        /// Calculates the gas usage of a rawTransaction by combining the intrinsic gas cost with
        /// the cost of a test run interacting with a potential contract
        /// </summary>
        /// <param name="transaction">The rawTransaction for which the cost is calculated</param>
        /// <param name="client"></param>
        /// <returns>The total gas cost of a transaction</returns>
        public static async Task<ulong> CalculateTotalGasCost(this Transaction transaction, VeChainClient client)
        {
            var executionCost = await transaction.CalculateExecutionGasCost(client);
            var intrinsicCost = transaction.CalculateIntrinsicGasCost();
            return executionCost + intrinsicCost;
        }

        /// <summary>
        /// Calculates the execution gas cost of a transaction by submitting it to the contract on the client
        /// instance of the blockchain.
        /// </summary>
        /// <param name="transaction">The Transaction for which the cost is calculated</param>
        /// <param name="client"></param>
        /// <returns>The execution gas cost of the transaction</returns>
        public static async Task<ulong> CalculateExecutionGasCost(this Transaction transaction, VeChainClient client)
        {
            var callResults = await client.ExecuteAddressCode(transaction.clauses);
            return (ulong) callResults.Sum(result => (decimal) result.gasUsed);
        }

        /// <summary>
        /// Calculates the intrinsic gas usage of a rawTransaction.
        /// </summary>
        /// <param name="transaction">The Transaction for which the cost is calculated</param>
        /// <returns>The intrinsic gas cost of the transaction</returns>
        public static ulong CalculateIntrinsicGasCost(this Transaction transaction)
        {
            if (transaction?.clauses == null)
                throw new NullReferenceException("Transaction is null");

            const uint txGas = 5000;
            const uint clauseGas = 16_000;
            const uint clauseGasContractCreation = 48_000;

            if (transaction.clauses.Length == 0)
                return txGas + clauseGas;

            // Add all the gas cost of the data written to the chain to either the 
            // gas cost of a clause or a contract creation based on whether the 'to'
            // value has been set
            ulong dataGasCost = 0;
            foreach (Clause transactionClause in transaction.clauses)
            {
                dataGasCost += transactionClause.data.DataGas();
                dataGasCost += transactionClause.to != null ? clauseGas : clauseGasContractCreation;
            }

            return txGas + dataGasCost;
        }

        /// <summary>
        /// Calculates the gas cost of the data part of a clause.
        /// </summary>
        /// <param name="data">The data for which the cost is calculated</param>
        /// <returns></returns>
        public static ulong DataGas(this string data)
        {
            const uint zgas = 4;
            const uint nzgas = 68;

            uint totalGas = 0;

            for (var i = 2; i < data.Length; i += 2)
                totalGas += data.Substring(i, 2) == "00" ? zgas : nzgas;

            return totalGas;
        }


        /// <summary>
        /// Calculates the transaction id of a signed transaction. Will throw an exception when the transaction
        /// is not signed. The id is not needed to submit a transaction but can be useful when transactions
        /// depend on each other.
        /// </summary>
        /// <param name="transaction">The RawTransaction for which the id needs to be calculated.</param>
        /// <param name="signer">The address of the signer.</param>
        /// <returns>The RawTransaction with the calculated id.</returns>
        public static RlpTransaction CalculateTxId(this RlpTransaction transaction, Address signer)
        {
            if (transaction.signature == null)
                throw new ArgumentException("transaction is not singed");

            // Hash the RLP encoded transaction
            var signingHash = Hash.HashBlake2B(transaction.RLPData);
            var signingAddress = signer.HexString.HexStringToByteArray();

            byte[] concatenatedBytes = new byte[52];
            Unsafe.CopyBlock(ref concatenatedBytes[0], ref signingHash[0], (uint) signingHash.Length);
            //Array.Copy(signingHash,    0, concatenatedBytes, 0, signingHash.Length);
            Unsafe.CopyBlock(ref concatenatedBytes[signingHash.Length], ref signingAddress[0], (uint) signingAddress.Length);
            //Array.Copy(signingAddress, 0, concatenatedBytes, signingHash.Length, signingAddress.Length);

            // Hash the bytes from the signed transaction and the signer address
            byte[] txIdBytes = Hash.HashBlake2B(concatenatedBytes);
            transaction.id = txIdBytes.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);
            return transaction;
        }

        /// <summary>
        /// Sign the transaction by RLP encoding it and calculating the signature of the resulting byte[].
        /// </summary>
        /// <param name="transaction">The transaction that will be signed.</param>
        /// <param name="key">The key with which the transaction will be signed.</param>
        /// <returns>The signed transaction</returns>
        public static RlpTransaction Sign(this RlpTransaction transaction, ECKeyPair key)
        {
            var rlp = transaction.RLPData;

            SignatureData signatureData = ECDSASign.SignMessage(rlp, key, true);
            transaction.signature = signatureData.ToByteArray();

            return transaction;
        }

        /// <summary>
        /// Transfer the transaction to the blockchain to be included in the next block.
        /// </summary>
        /// <param name="transaction">The transaction to be included in the blockchain</param>
        /// <param name="client">The client that determines the interaction with the blockchain</param>
        /// <returns>The result of the transfer</returns>
        public static async Task<TransferResult> Transfer(this RlpTransaction transaction, VeChainClient client)
        {
            if (transaction?.signature == null)
                throw new ArgumentException("Unsigned transaction can not be sent");

            var bytes = transaction.RLPData;

            //var transactionString = bytes.ByteArrayToString(StringType.Hex | StringType.ZeroLowerX);

            return await client.SendTransaction(bytes);
        }

        /// <summary>
        /// RLP encode the transaction and turn it into a byte[]
        /// </summary>
        /// <param name="transaction">The transaction that is to be encoded</param>
        /// <returns>The encoded transaction</returns>
        public static byte[] Encode(this Transaction transaction)
        {
            return RlpTransaction.CreateUnsigned(
                transaction.chainTag,
                transaction.blockRef,
                transaction.expiration,
                transaction.clauses.Select(c => new RlpClause(c.to, c.value, c.data, false)).ToArray(),
                (ulong) transaction.nonce.HexToBigInteger(false),
                transaction.gasPriceCoef,
                transaction.gas,
                transaction.dependsOn
            ).RLPData;
        }
    }
}