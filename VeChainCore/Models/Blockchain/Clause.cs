using System.Runtime.Serialization;
using Nethereum.RLP;

namespace VeChainCore.Models.Blockchain
{
    public abstract class Clause : IRLPElement
    {
        public abstract string to { get; }
        public abstract decimal value { get; }
        public abstract string data { get; }

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