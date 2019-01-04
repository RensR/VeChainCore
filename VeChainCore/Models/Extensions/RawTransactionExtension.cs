using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using VeChainCore.Models.Transaction;
using VeChainCore.Logic.Cryptography;
using System.Numerics;
using System.Text;
using VeChainCore.Logic;
using VeChainCore.Models.Keys;

namespace VeChainCore.Models.Extensions
{
    public static class RawTransactionExtension
    {
        public static string CalculateTxId(this RawTransaction rawTransaction, string signer)
        {
            if (rawTransaction.signature == RLP.EMPTY_BYTE_ARRAY)
                throw new ArgumentException("rawTransaction is not singed");

            var rlp = rawTransaction.AsRlpValues();
            var rlpCollection = new RLPCollection();
            rlpCollection.AddRange(rlp);
            var transactionFields = RlpEncoder.Encode(rlpCollection);
            
            var signingHash = Hash.HashBlake2B(transactionFields);


            byte[] concatenatedBytes = new byte[52];
            Buffer.BlockCopy(signingHash, 0, concatenatedBytes, 0, signingHash.Length);
            Buffer.BlockCopy(signer.ToBytesForRLPEncoding(), 0, concatenatedBytes, signingHash.Length, signer.ToBytesForRLPEncoding().Length);
            byte[] txIdBytes = Hash.HashBlake2B(concatenatedBytes);
            return txIdBytes.ToHex(true);


            // ????
            var signerAddress = new EthereumMessageSigner().EcRecover(signingHash, rawTransaction.signature.ToString());
            var txId = Hash.HashBlake2B(signingHash.Concat(signerAddress.HexToByteArray()));

            return "";
        }


        public static RawTransaction Sign(this RawTransaction rawTransaction, ECKeyPair key)
        {
            var rlp = new RLPCollection();
            rlp.AddRange(rawTransaction.AsRlpValues());

            byte[] o = Format(rlp).HexToByteArray();

            var result = SignMessage(o, key, false);

            return rawTransaction;

        }

        public static ECDSASignature SignMessage(byte[] message, ECKeyPair keyPair, bool needToHash)
        {

            BigInteger publicKey = Hex.HexToBigInt(keyPair.address);
            var messageHash = needToHash ? Hash.HashBlake2B(message) : message;

            int recId = -1;

            ECDSASignature sig = keyPair.ECKey.Sign(messageHash);
            for (int i = 0; i < 4; i++)
            {
                ECKey k = ECKey.RecoverFromSignature(i, sig, messageHash, false);
                if (k != null && k.Equals(keyPair.ECKey))
                {
                    recId = i;
                    break;
                }
            }

            if (recId == -1)
            {
                throw new Exception("Sign the data failed.");
            }

            if (recId == 2 || recId == 3)
            {
                throw new Exception("Recovery is not valid for VeChain MainNet.");
            }

            return sig;

            //byte v = (byte) recId;
            //byte[] r = BytesUtils.toBytesPadded(sig.r, 32);
            //byte[] s = BytesUtils.toBytesPadded(sig.s, 32);

            //return new ECDSASign.SignatureData(v, r, s);

        }

        public static List<IRLPElement> AsRlpValues(this RawTransaction rawTransaction)
        {
            if (rawTransaction.reserved != null)
                throw new ConstraintException("Reserved is not supported");

            var rlpList = new List<IRLPElement>
            {
                new RLPItem(rawTransaction.chainTag),
                new RLPItem(rawTransaction.blockRef),
                new RLPItem(rawTransaction.expiration),
                GetClauseRlp(rawTransaction.clauses),
                new RLPItem(rawTransaction.gasPriceCoef),
                new RLPItem(rawTransaction.gas),
                new RLPItem(rawTransaction.nonce),
                new RLPCollection(),
                new RLPItem(rawTransaction.signature)
            };

            return rlpList;
        }

        public static RLPCollection GetClauseRlp(byte[][][] clauses)
        {
            var rlpClauses = new RLPCollection();

            foreach (var clause in clauses)
            {
                var singleClause = new RLPCollection();
                singleClause.AddRange(clause.Select(clauseItem => new RLPItem(clauseItem)));
                rlpClauses.Add(singleClause);
            }

            return rlpClauses;
        }

        public static string Format(IRLPElement element)
        {
            var output = new StringBuilder();
            switch (element)
            {
                case null:
                    throw new Exception("RLPElement object can't be null");
                case RLPCollection rlpCollection:
                {
                    output.Append("[");
                    foreach (var innerElement in rlpCollection)
                        Format(innerElement);
                    output.Append("]");
                    break;
                }
                default:
                    output.Append(element.RLPData.ToHex() + ", ");
                    break;
            }

            return output.ToString();
        }
    }
}
