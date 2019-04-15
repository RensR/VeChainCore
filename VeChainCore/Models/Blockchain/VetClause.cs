using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Org.BouncyCastle.Math;
using VeChainCore.Models.Core;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;

namespace VeChainCore.Models.Blockchain
{
    public class VetClause : IEquatable<VetClause>, IRLPElement
    {
        private readonly VET _vet;
        
        public string to { get; }
        public decimal value { get; }
        public string data { get; }

        public VetClause(string to, decimal value, string data)
        {
            this.to = to;
            this.value = value;
            this.data = data;
            
            _vet = new VET(this.value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VetClause);
        }

        public bool Equals(VetClause other)
        {
            return other != null &&
                   to == other.to &&
                   value == other.value &&
                   data == other.data;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(to, value, data);
        }

        public static bool operator ==(VetClause clause1, VetClause clause2)
        {
            return EqualityComparer<VetClause>.Default.Equals(clause1, clause2);
        }

        public static bool operator !=(VetClause clause1, VetClause clause2)
        {
            return !(clause1 == clause2);
        }

        [IgnoreDataMember]
        public byte[][] RlpDataParts
            => new[]
            {
                new Address(to).RLPData,
                RLP.EncodeElement(_vet.AsBytes),
                RLP.EncodeElement(data.HexToByteArray())
            };

        [IgnoreDataMember]
        public byte[] RLPData => RLP.EncodeList(RlpDataParts);
    }
}