using System;
using Nethereum.Signer.Crypto;
using System.Numerics;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using VeChainCore.Models.Core;
using VeChainCore.Utils.Rlp;
using VeChainCore.Utils.Cryptography;
using VeChainCore.Utils;

namespace VeChainCore.Models.Extensions
{
    public static class RawTransactionExtension
    {
        public static string CalculateTxId(this RawTransaction transaction, Address signer)
        {
            if (transaction.signature == null)
                throw new ArgumentException("transaction is not singed");

            var transactionFields = RlpEncoder.Encode(new RlpTransaction(transaction).AsRLPValues());

            var signingHash = Hash.HashBlake2B(transactionFields);


            byte[] concatenatedBytes = new byte[52];
            Buffer.BlockCopy(signingHash, 0, concatenatedBytes, 0, signingHash.Length);
            Buffer.BlockCopy(signer.HexString.HexStringToByteArray(), 0, concatenatedBytes, signingHash.Length, signer.HexString.HexStringToByteArray().Length);
            byte[] txIdBytes = Hash.HashBlake2B(concatenatedBytes);
            return txIdBytes.ByteArrayToString(StringType.Hex, Prefix.ZeroLowerX);
        }


        public static RawTransaction Sign(this RawTransaction transaction, ECKeyPair key)
        {
            var rlp = RlpEncoder.Encode(new RlpTransaction(transaction).AsRLPValues());

            var sigdata = ECDSASign.signMessage(rlp, key, false);

            
            //byte[] o = Format(rlp).HexToByteArray();

            //var result = SignMessage(o, key, false);

            return transaction;

        }
    }
}
