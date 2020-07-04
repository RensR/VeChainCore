using VeChainCore.Models.Blockchain;

namespace VeChainCore.Models.Extensions
{
    public static class ClauseExtensions
    {
        /// <summary>
        /// Calculates the gas cost of the data part of a clause.
        /// </summary>
        /// <param name="clause">The clause for which the cost is calculated</param>
        /// <returns></returns>
        public static ulong CalculateDataGas(this IClause clause)
        {
            const uint zgas = 4;
            const uint nzgas = 68;

            uint totalGas = 0;

            var data = clause.data;

            for (int i = 2; i < data.Length; i += 2)
            {
                string hexPair = data.Substring(i, 2);
                totalGas += hexPair == "00" ? zgas : nzgas;
            }

            return totalGas;
        }
    }
}