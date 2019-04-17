using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    public abstract partial class Clause
    {
        [IgnoreDataMember]
        public abstract byte[] RLPData { get; }

        /// <summary>
        /// Calculates the gas cost of the data part of a clause.
        /// </summary>
        /// <param name="data">The data for which the cost is calculated</param>
        /// <returns></returns>
        public ulong CalculateDataGas()
        {
            const uint zgas = 4;
            const uint nzgas = 68;

            uint totalGas = 0;

            for (int i = 2; i < data.Length; i += 2)
            {
                string hexPair = data.Substring(i, 2);
                totalGas += hexPair == "00" ? zgas : nzgas;
            }

            return totalGas;
        }
    }
}