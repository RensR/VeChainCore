using System;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Models.Extensions
{
    public static class TransactionExtension
    {
        /// <summary>
        /// Calculates the gas usage of a rawTransaction. Calculations are taken from the Thor Devkit
        /// </summary>
        /// <param name="transaction">The rawTransaction for which the cost is calculated</param>
        /// <returns></returns>
        public static ulong CalculateGasCost(this Transaction transaction)
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
                dataGasCost += transactionClause.DataGas();
                dataGasCost += transactionClause.to != null ? clauseGas : clauseGasContractCreation;
            }

            return txGas +dataGasCost;
        }

        /// <summary>
        /// Calculates the gas cost of the data part of a clause.
        /// </summary>
        /// <param name="data">The data for which the cost is calculated</param>
        /// <returns></returns>
        public static ulong DataGas(string data)
        {
            const uint zgas = 4;
            const uint nzgas = 68;

            uint totalGas = 0;

            for (var i = 2; i < data.Length; i += 2)
                totalGas += data.Substring(i, 2) == "00" ? zgas : nzgas;

            return totalGas;
        }

        public static ulong DataGas(this Clause clause)
        {
            return DataGas(clause.data);
        }
    }
}
