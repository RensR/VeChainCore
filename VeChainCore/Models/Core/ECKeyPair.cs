using Nethereum.Signer.Crypto;

namespace VeChainCore.Core
{
    public class ECKeyPair
    {
        public ECKey ECKey { get; set; }
        public string privateKey { get; set; }
        public string address { get; set; }
    }
}
