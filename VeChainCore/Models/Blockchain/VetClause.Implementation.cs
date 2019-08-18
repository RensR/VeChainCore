using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using VeChainCore.Models.Core;

namespace VeChainCore.Models.Blockchain
{
    public partial class VetClause : Clause, IEquatable<VetClause>, IRLPElement
    {
        public VetClause(string to, decimal value, string data)
        {
            this.to = to;
            this.value = value;
            this.data = data;
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
                RLP.EncodeElement(new VET(value).AsBytes),
                RLP.EncodeElement(data.HexToByteArray())
            };

        [IgnoreDataMember]
        public override byte[] RLPData => RLP.EncodeList(RlpDataParts);

        protected override string GetTo() => to;
        protected override decimal GetValue() => value;
        protected override string GetData() => data;
    }
}