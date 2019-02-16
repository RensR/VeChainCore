namespace VeChainCore.Models.Blockchain
{
    public class CallResult
    {
        public string data { get; set; }
        public Event[] events { get; set; }
        public Transfer[] transfers { get; set; }
        public ulong gasUsed { get; set; }
        public bool reverted { get; set; }
        public string vmError { get; set; }
    }
}
