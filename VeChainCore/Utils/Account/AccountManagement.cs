using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using VeChainCore.Models.Keys;

namespace VeChainCore.Utils.Account
{
    // As VeChain uses the same key generation as Ethereum we use the Nethereum implementation for most
    // logic in this class.
    public class AccountManagement
    {
        public static ECKeyPair CreateNewPrivateKey()
        {
            var ecKey = EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKey();
            var genAddress = ecKey.GetPublicAddress();

            return new ECKeyPair
            {
                privateKey = privateKey,
                address = genAddress
            };
        }



    }
}
