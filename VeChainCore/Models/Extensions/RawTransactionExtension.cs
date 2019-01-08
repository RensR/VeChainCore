using System;
using Nethereum.Signer.Crypto;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using VeChainCore.Core;
using VeChainCore.Models.Core;
using VeChainCore.Utils.Rlp;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;

namespace VeChainCore.Models.Extensions
{
    public static class RawTransactionExtension
    {
        public static string CalculateTxId(this RawTransaction transaction, string signer)
        {
            if (transaction.signature == null)
                throw new ArgumentException("transaction is not singed");

            var transactionFields = RlpEncoder.Encode(new RlpTransaction(transaction).AsRLPValues());

            var signingHash = Hash.HashBlake2B(transactionFields);


            byte[] concatenatedBytes = new byte[52];
            Buffer.BlockCopy(signingHash, 0, concatenatedBytes, 0, signingHash.Length);
            Buffer.BlockCopy(signer.HexStringToByteArray(), 0, concatenatedBytes, signingHash.Length, signer.HexStringToByteArray().Length);
            byte[] txIdBytes = Hash.HashBlake2B(concatenatedBytes);
            return txIdBytes.ToHex(true);


            // ????
            var signerAddress = new EthereumMessageSigner().EcRecover(signingHash, transaction.signature.ToString());
            var txId = Hash.HashBlake2B(signingHash.Concat(signerAddress.HexToByteArray()));

            return "";
        }


        public static RawTransaction Sign(this RawTransaction transaction, ECKeyPair key)
        {
            var rlp = RlpEncoder.Encode(new RlpTransaction(transaction).AsRLPValues());

            
            //byte[] o = Format(rlp).HexToByteArray();

            //var result = SignMessage(o, key, false);

            return transaction;

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
    }
}
