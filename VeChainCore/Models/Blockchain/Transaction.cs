using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Math;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils.Cryptography;

namespace VeChainCore.Models.Blockchain
{
    public class Transaction : IEquatable<Transaction>, IRLPElement
    {
        private static readonly byte[] Reserved = new byte[1] {0xc0};

        public Transaction(
            byte chainTag,
            string blockRef,
            uint expiration,
            VetClause[] clauses,
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

        public byte chainTag { get; set; }
        public string blockRef { get; set; }
        public uint expiration { get; set; }
        public VetClause[] clauses { get; set; }
        public byte gasPriceCoef { get; set; }
        public ulong gas { get; set; }
        public string dependsOn { get; set; }
        public ulong nonce { get; set; }


        public string signature { get; set; }


        public string id { get; set; }
        public string origin { get; set; }
        public ulong size { get; set; }
        public TxMeta meta { get; set; }

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
                   EqualityComparer<VetClause[]>.Default.Equals(clauses, other.clauses) &&
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
                RLP.EncodeByte(chainTag),
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
                        key.PublicKey.ToByteArray()
                            .PadLeading(32)
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
    }
}