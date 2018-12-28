using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using VeChainCore.Models.Keys;

namespace VeChainCore.Logic.Account
{
    // As VeChain uses the same key generation as ethereum we use the Nethereum implementation for most
    // logic in this class.
    public class AccountManagement
    {
        public static PrivateKey CreateNewPrivateKey()
        {
            var ecKey = EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKey();
            var genAddress = ecKey.GetPublicAddress();

            return new PrivateKey
            {
                privateKey = privateKey,
                address = genAddress
            };
        }



    }
}
