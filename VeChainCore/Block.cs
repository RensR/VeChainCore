namespace VeChainCore
{
    public class Block
    {
        public uint number;
        public string id;
        public uint size;
        public string parentID;
        public uint timestamp;
        public uint gasLimit;
        public string beneficiary;
        public uint gasUsed;
        public uint totalScore;
        public string txsRoot;
        public string stateRoot;
        public string receiptsRoot;
        public string signer;
        public bool isTrunk;
        public Transaction[] transactions;

        public override string ToString()
        {
            string block = $"Block: {number}";
            return base.ToString();
        }
    }
}
