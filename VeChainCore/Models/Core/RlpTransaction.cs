using System;
using System.Linq;
using System.Text;
using Nethereum.RLP;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Extensions;
using VeChainCore.Utils;
using VeChainCore.Utils.Cryptography;

namespace VeChainCore.Models.Core
{
    public class RlpTransaction : IRLPElement
    {
        // use CreateUnsigned
        private RlpTransaction()
        {
        }

        private static readonly byte[] Reserved = new byte[1] {0xc0};

        public readonly byte[][] RLPDataParts = new byte[10][]
        {
            Array.Empty<byte>(), // chainTag
            Array.Empty<byte>(), // blockRef
            Array.Empty<byte>(), // expiration
            Array.Empty<byte>(), // clauses
            Array.Empty<byte>(), // gasPriceCoef
            Array.Empty<byte>(), // gas
            Array.Empty<byte>(), // dependsOn
            Array.Empty<byte>(), // nonce
            Reserved,
            Array.Empty<byte>() // signature
        };

        public byte[] RLPData => RLP.EncodeList(RLPDataParts);
        public byte[] RLPSignatureData => RLP.EncodeList(RLPDataParts.Take(9).ToArray());

        private byte _chainTag;
        private string _blockRef;
        private uint _expiration;
        private RlpClause[] _clauses;
        private byte _gasPriceCoef;
        private ulong _gas;
        private string _dependsOn;
        private ulong _nonce;
        private byte[] _signature;

        public string id { get; set; }
        public string origin { get; set; }
        public ulong size { get; set; }
        public TxMeta meta { get; set; }

        public byte chainTag
        {
            get => _chainTag;
            set
            {
                _chainTag = value;
                RLPDataParts[0] = RLP.EncodeByte(value);
            }
        }

        public string blockRef
        {
            get => _blockRef;
            set
            {
                _blockRef = value;
                RLPDataParts[1] = RLP.EncodeElement(value.HexStringToByteArray().TrimLeadingZeroBytes());
            }
        }

        public uint expiration
        {
            get => _expiration;
            set
            {
                _expiration = value;
                RLPDataParts[2] = RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(value));
            }
        }

        public RlpClause[] clauses
        {
            get => _clauses;
            set
            {
                _clauses = value;
                RLPDataParts[3] = RLP.EncodeList(clauses.Select(c => c.RLPData).ToArray().Flatten());
            }
        }

        public byte gasPriceCoef
        {
            get => _gasPriceCoef;
            set
            {
                _gasPriceCoef = value;
                RLPDataParts[4] = RLP.EncodeByte(value);
            }
        }

        public ulong gas
        {
            get => _gas;
            set
            {
                _gas = value;
                RLPDataParts[5] = RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(value));
            }
        }

        public string dependsOn
        {
            get => _dependsOn;
            set
            {
                _dependsOn = value;
                RLPDataParts[6] = RLP.EncodeElement(value?.HexStringToByteArray().TrimLeadingZeroBytes());
            }
        }

        public ulong nonce
        {
            get => _nonce;
            set
            {
                _nonce = value;
                RLPDataParts[7] = RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesForRLPEncoding(value));
            }
        }

        public byte[] signature
        {
            get
            {
                return _signature;
            }
            set
            {
                _signature = value;
                RLPDataParts[9] = RLP.EncodeElement(value);
            }
        }

        public byte[] Sign()
        {
            return _signature = Hash.HashBlake2B(RLPSignatureData);
        }

        public static RlpTransaction CreateUnsigned(
            byte chainTag,
            string blockRef,
            uint expiration,
            RlpClause[] clauses,
            ulong nonce,
            byte gasPriceCoef = 100,
            ulong gas = 21000,
            string dependsOn = "")
        {
            if (clauses == null || clauses.Length < 1)
                throw new ArgumentException("No clauses found");

            return new RlpTransaction
            {
                chainTag = chainTag,
                blockRef = blockRef,
                expiration = expiration,
                clauses = clauses,
                gasPriceCoef = gasPriceCoef,
                gas = gas,
                dependsOn = dependsOn,
                nonce = nonce,
                signature = null
            };
        }
    }
}