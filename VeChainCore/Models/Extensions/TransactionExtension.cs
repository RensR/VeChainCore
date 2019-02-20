using System;
using System.Threading.Tasks;
using VeChainCore.Client;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Models.Extensions
{
    public static class TransactionExtension
    {
        /// <summary>
        /// Calculates the gas usage of a rawTransaction by combining the intrinsic gas cost with
        /// the cost of a test run interacting with a potential contract
        /// <param name="transaction">The rawTransaction for which the cost is calculated</param>
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
            ulong totalExecutionGas = 0;


            var callResult = await client.ExecuteAddressCode(transaction.clauses);


            return totalExecutionGas;
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
    }
}
