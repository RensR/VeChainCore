using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Utf8Json;
using VeChainCore.Client;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;

namespace VeChainCore.Models.Blockchain
{
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    public partial class Transaction : IEquatable<Transaction>, IRLPElement
    {
        public bool Equals(Transaction other)
        {
            if (other == null)
                return false;
            return id == other.id
                   || (
                       chainTag == other.chainTag
                       && blockRef == other.blockRef
                       && expiration == other.expiration
                       && clauses.SequenceEqual(other.clauses)
                       && gasPriceCoef == other.gasPriceCoef
                       && gas == other.gas
                       && nonce == other.nonce
                       && dependsOn == other.dependsOn
                   );
        }


        public byte[][] GetRlpDataElements()
        {
            return new[]
            {
                RLP.EncodeByte((byte) chainTag),
                RLP.EncodeElement(blockRef.ToBigEndianBytes().TrimLeading()),
                RLP.EncodeElement(((long) expiration).ToBytesForRLPEncoding().TrimLeading()),
                RLP.EncodeList(clauses.Select(c => c.RLPData).ToArray()),
                RLP.EncodeByte(gasPriceCoef),
                RLP.EncodeElement(gas.ToBigEndianBytes().TrimLeading()),
                RLP.EncodeElement(dependsOn?.HexToByteArray().TrimLeading()),
                RLP.EncodeElement(nonce.ToBigEndianBytes().TrimLeading()),
                Reserved,
                RLP.EncodeElement(signature?.HexToByteArray())
            };
        }

        public Transaction(
            Network chainTag,
            ulong blockRef,
            uint expiration,
            IEnumerable<Clause> clauses,
            ulong nonce,
            byte gasPriceCoef,
            ulong gas,
            string dependsOn
        )
            : this(
                chainTag,
                blockRef,
                expiration,
                clauses as List<Clause> ?? new List<Clause>(clauses),
                nonce,
                gasPriceCoef,
                gas,
                dependsOn
            )
        {
        }

        public Transaction(
            Network chainTag,
            ulong blockRef,
            uint expiration,
            List<Clause> clauses,
            ulong nonce,
            byte gasPriceCoef,
            ulong gas,
            string dependsOn
        )
        {
            this.chainTag = chainTag;
            this.blockRef = blockRef;
            this.expiration = expiration;
            this.clauses = clauses;
            this.nonce = nonce;
            this.gasPriceCoef = gasPriceCoef;
            this.gas = gas;
            this.dependsOn = dependsOn;
        }

        public byte[][] GetRlpDataSignatureElements()
            => GetRlpDataElements().Take(9).ToArray();

        [IgnoreDataMember]
        public byte[] RLPData
            => RLP.EncodeList(GetRlpDataElements());

        [IgnoreDataMember]
        public byte[] RlpDataForSignature
            => RLP.EncodeList(GetRlpDataSignatureElements());

        public Transaction Sign(ECKeyPair key)
        {
            var hash = Hash.Blake2B(this.RlpDataForSignature);

            var sig = ECDSASign.SignMessage(hash, key, false);

            var sigBytes = sig.ToByteArray();
            signature = sigBytes.ToHex(true);

            var signer
                = Hash.Keccak256(
                        key.PublicKey.ToByteArrayUnsigned()
                            .PadLeading(20)
                    )
                    .LastBytes(20);


            byte[] concatenatedBytes = new byte[52];
            Unsafe.CopyBlock(ref concatenatedBytes[0], ref hash[0], (uint) hash.Length);
            Unsafe.CopyBlock(ref concatenatedBytes[hash.Length], ref signer[0], (uint) signer.Length);

            byte[] txIdBytes = Hash.Blake2B(concatenatedBytes);
            id = txIdBytes.ToHex(true);

            return this;
        }

        public byte[] GetSignature()
            => Hash.Blake2B(RlpDataForSignature);

        /// <summary>
        /// Calculates the gas usage of a rawTransaction by combining the intrinsic gas cost with
        /// the cost of a test run interacting with a potential contract
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The total gas cost of a transaction</returns>
        public async Task<ulong> CalculateTotalGasCost(IVeChainClient client)
        {
            var executionCost = await CalculateExecutionGasCost(client);
            var intrinsicCost = CalculateIntrinsicGasCost();
            return executionCost + intrinsicCost;
        }

        /// <summary>
        /// Calculates the execution gas cost of a transaction by submitting it to the contract on the client
        /// instance of the blockchain.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The execution gas cost of the transaction</returns>
        public async Task<ulong> CalculateExecutionGasCost(IVeChainClient client)
        {
            var callResults = await client.ExecuteAddressCode(clauses);
            return (ulong) callResults.Sum(result => (decimal) result.gasUsed);
        }

        /// <summary>
        /// Calculates the intrinsic gas usage of a rawTransaction.
        /// </summary>
        /// <returns>The intrinsic gas cost of the transaction</returns>
        public ulong CalculateIntrinsicGasCost()
        {
            if (clauses == null)
                throw new NullReferenceException("Transaction is null");

            const uint txGas = 5000;
            const uint clauseGas = 16_000;
            const uint clauseGasContractCreation = 48_000;

            if (clauses.Count == 0)
                return txGas + clauseGas;

            // Add all the gas cost of the data written to the chain to either the 
            // gas cost of a clause or a contract creation based on whether the 'to'
            // value has been set
            ulong dataGasCost = 0;
            foreach (IClause transactionClause in clauses)
            {
                dataGasCost += transactionClause.CalculateDataGas();
                dataGasCost += transactionClause.to != null ? clauseGas : clauseGasContractCreation;
            }

            return txGas + dataGasCost;
        }

        public override int GetHashCode()
        {
            var h = new HashCode();
            h.Add(id);
            h.Add(chainTag);
            h.Add(blockRef);
            h.Add(expiration);
            h.Add(clauses);
            h.Add(gasPriceCoef);
            h.Add(gas);
            h.Add(nonce);
            h.Add(dependsOn);
            h.Add(signature);
            return h.ToHashCode();
        }

        public override bool Equals(object obj)
            => Equals(obj as Transaction);

        public static bool operator ==(Transaction clause1, Transaction clause2)
            => EqualityComparer<Transaction>.Default.Equals(clause1, clause2);

        public static bool operator !=(Transaction clause1, Transaction clause2)
            => !(clause1 == clause2);

        public void AddClause(Clause item) => clauses.Add(item);

        public void ClearClauses() => clauses.Clear();

        public bool ContainsClause(Clause item) => clauses.Contains(item);

        public bool RemoveClause(Clause item) => clauses.Remove(item);

        public int IndexOfClause(Clause item) => clauses.IndexOf(item);

        public void InsertClause(int index, Clause item) => clauses.Insert(index, item);

        public void RemoveAtClause(int index) => clauses.RemoveAt(index);

        public override string ToString()
        {
            return id != null
                ? $"{{\"id\":{id}}}"
                : JsonSerializer.ToJsonString(this, VeChainClient.JsonFormatterResolver);
        }
    }
}