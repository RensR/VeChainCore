using System;
using System.Collections.Generic;
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

            // Add all the gas cost of the data writen to the chain to either the 
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

        public static List<IRLPElement> Encode(this RawTransaction rawTransaction)
        {
            if (rawTransaction.reserved != null)
                throw new ArgumentNullException("reservedList is not supported");

            var RLPList = new List<IRLPElement>
            {
                new RLPItem(rawTransaction.chainTag),
                new RLPItem(rawTransaction.blockRef),
                new RLPItem(rawTransaction.expiration),
                GetClauseRLP(rawTransaction.clauses),
                new RLPItem(rawTransaction.gasPriceCoef),
                new RLPItem(rawTransaction.gas),
                new RLPItem(rawTransaction.nonce),
                new RLPCollection(),
                new RLPItem(rawTransaction.signature)
            };     

            return RLPList;
        }

        public static byte[][][] GetRawClauses(this Clause[] clauses)
        {
            var clausesAsBytes = new byte[clauses.Length][][];

            for (var i = 0; i < clauses.Length; i++)
            {
                var clause = clauses[i];

                var clauseArray = new byte[3][];

                clauseArray[0] = clause.to == null
                    ? RLP.EMPTY_BYTE_ARRAY
                    : clauseArray[0] = clause.to.ToBytesForRLPEncoding();

                clauseArray[1] = clause.value == null
                    ? RLP.EMPTY_BYTE_ARRAY
                    : clauseArray[0] = clause.value.ToBytesForRLPEncoding();

                clauseArray[2] = clause.data == null
                    ? RLP.EMPTY_BYTE_ARRAY
                    : clauseArray[0] = clause.data.ToBytesForRLPEncoding();

                clausesAsBytes[i] = clauseArray;
            }
            return clausesAsBytes;
        }

        public static RLPCollection GetClauseRLP(byte[][][] clauses)
        {
            var rlpClauses = new RLPCollection();

            foreach (var clause in clauses)
            {
                var singleClause = new RLPCollection();
                singleClause.AddRange(clause.Select(clauseItem => new RLPItem(clauseItem)));
                rlpClauses.Add(singleClause);
            }

            return rlpClauses;
        }
    }
}
