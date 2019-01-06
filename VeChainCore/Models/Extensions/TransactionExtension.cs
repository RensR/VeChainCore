using System;
using System.Linq;
using Nethereum.RLP;
using VeChainCore.Models.Transaction;

namespace VeChainCore.Models.Extensions
{
    public static class TransactionExtension
    {
        /// <summary>
        /// Calculates the gas usage of a rawTransaction. Calculations are taken from the Thor Devkit
        /// </summary>
        /// <param name="transaction">The rawTransaction for which the cost is calculated</param>
        /// <returns></returns>
        public static long CalculateGasCost(this Transaction.Transaction transaction)
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
            return txGas + transaction.clauses.Sum(clause => clause.DataGas() +
                                         (clause.to != null ? clauseGas : clauseGasContractCreation));
        }

        /// <summary>
        /// Calculates the gas cost of the data part of a clause.
        /// </summary>
        /// <param name="data">The data for which the cost is calculated</param>
        /// <returns></returns>
        public static uint DataGas(string data)
        {
            const uint zgas = 4;
            const uint nzgas = 68;

            uint totalGas = 0;

            for (var i = 2; i < data.Length; i += 2)
                totalGas += data.Substring(i, 2) == "00" ? zgas : nzgas;

            return totalGas;
        }

        public static uint DataGas(this Clause clause)
        {
            return DataGas(clause.data);
        }
    }
}
