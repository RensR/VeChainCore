using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Client;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils.Cryptography;

namespace VeChainCore.Models.Blockchain
{
    public partial class Transaction : IEquatable<Transaction>, IRLPElement
    {
        public override string ToString()
        {
            return $"Transaction {id}";
        }

        public bool Equals(Transaction other)
        {
            return other != null &&
                   id == other.id &&
                   chainTag == other.chainTag &&
                   blockRef == other.blockRef &&
                   expiration == other.expiration &&
                   EqualityComparer<Clause[]>.Default.Equals(clauses, other.clauses) &&
                   gasPriceCoef == other.gasPriceCoef &&
                   gas == other.gas &&
                   origin == other.origin &&
                   nonce == other.nonce &&
                   dependsOn == other.dependsOn &&
                   size == other.size &&
                   EqualityComparer<TxMeta>.Default.Equals(meta, other.meta);
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
            h.Add(origin);
            h.Add(nonce);
            h.Add(dependsOn);
            h.Add(size);
            h.Add(meta);
            h.Add(signature);
            return h.ToHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Transaction);
        }

        public static bool operator ==(Transaction clause1, Transaction clause2)
        {
            return EqualityComparer<Transaction>.Default.Equals(clause1, clause2);
        }

        public static bool operator !=(Transaction clause1, Transaction clause2)
        {
            return !(clause1 == clause2);
        }

        public byte[][] GetRlpDataElements()
        {
            var nbiGas = (System.Numerics.BigInteger) gas;
            var nbiNonce = (System.Numerics.BigInteger) nonce;

            return new[]
            {
                RLP.EncodeByte((byte) chainTag),
                RLP.EncodeElement(blockRef.HexToByteArray().TrimLeading()),
                RLP.EncodeElement(((long) expiration).ToBytesForRLPEncoding().TrimLeading()),
                RLP.EncodeList(clauses.Select(c => c.RLPData).ToArray()),
                RLP.EncodeByte(gasPriceCoef),
                RLP.EncodeElement(nbiGas.ToBytesForRLPEncoding().TrimLeading()),
                RLP.EncodeElement(dependsOn?.HexToByteArray().TrimLeading()),
                RLP.EncodeElement(nbiNonce.ToBytesForRLPEncoding().TrimLeading()),
                Reserved,
                RLP.EncodeElement(signature?.HexToByteArray())
            };
        }

        public Transaction(
            Network chainTag,
            string blockRef,
            uint expiration,
            Clause[] clauses,
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
        {
            return GetRlpDataElements().Take(9).ToArray();
        }

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
        /// <param name="transaction">The rawTransaction for which the cost is calculated</param>
        /// <param name="client"></param>
        /// <returns>The total gas cost of a transaction</returns>
        public async Task<ulong> CalculateTotalGasCost(VeChainClient client)
        {
            var executionCost = await CalculateExecutionGasCost(client);
            var intrinsicCost = CalculateIntrinsicGasCost();
            return executionCost + intrinsicCost;
        }

        /// <summary>
        /// Calculates the execution gas cost of a transaction by submitting it to the contract on the client
        /// instance of the blockchain.
        /// </summary>
        /// <param name="transaction">The Transaction for which the cost is calculated</param>
        /// <param name="client"></param>
        /// <returns>The execution gas cost of the transaction</returns>
        public async Task<ulong> CalculateExecutionGasCost(VeChainClient client)
        {
            var callResults = await client.ExecuteAddressCode(clauses);
            return (ulong) callResults.Sum(result => (decimal) result.gasUsed);
        }

        /// <summary>
        /// Calculates the intrinsic gas usage of a rawTransaction.
        /// </summary>
        /// <param name="transaction">The Transaction for which the cost is calculated</param>
        /// <returns>The intrinsic gas cost of the transaction</returns>
        public ulong CalculateIntrinsicGasCost()
        {
            if (clauses == null)
                throw new NullReferenceException("Transaction is null");

            const uint txGas = 5000;
            const uint clauseGas = 16_000;
            const uint clauseGasContractCreation = 48_000;

            if (clauses.Length == 0)
                return txGas + clauseGas;

            // Add all the gas cost of the data written to the chain to either the 
            // gas cost of a clause or a contract creation based on whether the 'to'
            // value has been set
            ulong dataGasCost = 0;
            foreach (var transactionClause in clauses)
            {
                dataGasCost += transactionClause.CalculateDataGas();
                dataGasCost += transactionClause.to != null ? clauseGas : clauseGasContractCreation;
            }

            return txGas + dataGasCost;
        }
    }
}