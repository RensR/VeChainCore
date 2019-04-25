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
    public partial class VthoTransferClause : Clause, IEquatable<VthoTransferClause>, IRLPElement
    {
        private static readonly byte[] ZeroValue = BigInteger.Zero.ToByteArrayUnsigned().TrimLeading();

        public VthoTransferClause()
        {
            data = InitialData;
        }

        public VthoTransferClause(string data)
        {
            if (data.Length != InitialData.Length)
                throw new ArgumentException("Incorrect length.", nameof(data));

            if (data.Substring(0, 10) != TransferMethodId)
                throw new ArgumentException("Not a VTHO transfer method.", nameof(data));

            this.data = data;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VthoTransferClause);
        }

        public bool Equals(VthoTransferClause other)
        {
            return other != null &&
                   data == other.data;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(data);
        }

        public static bool operator ==(VthoTransferClause clause1, VthoTransferClause clause2)
        {
            return EqualityComparer<VthoTransferClause>.Default.Equals(clause1, clause2);
        }

        public static bool operator !=(VthoTransferClause clause1, VthoTransferClause clause2)
        {
            return !(clause1 == clause2);
        }

        [IgnoreDataMember]
        public byte[][] RlpDataParts
            => new[]
            {
                new Address(ContractAddress).RLPData,
                RLP.EncodeElement(ZeroValue),
                RLP.EncodeElement(data.HexToByteArray())
            };

        [IgnoreDataMember]
        public override byte[] RLPData => RLP.EncodeList(RlpDataParts);

        private static readonly string InitialData = TransferMethodId + new string('0', 128);
        protected override string GetTo() => ContractAddress;
        protected override decimal GetValue() => 0;
        protected override string GetData() => data;


        private string GetToField()
            => "0x" + data.Substring(6, 32);

        private void SetToField(string value)
            => data = value == null || !value.StartsWith("0x")
                ? throw new ArgumentException("Value must be a hex string.", nameof(value))
                : data.Substring(0, 10) + value.Substring(2, 64).PadLeft(64, '0') + data.Substring(74);

        private decimal GetValueField()
            => data.Substring(6, 32).HexToByteArray().ToBigInteger().ToDecimal() / VTHO.Unit.DecimalsMultiplier;

        private void SetValueField(decimal value)
            => data = value < 0
                ? throw new ArgumentOutOfRangeException("Value must be zero or positive.", nameof(value))
                : data.Substring(0, 74) + value.ToBigInteger().ToByteArrayUnsigned().ToHex().PadLeft(64, '0');
    }
}