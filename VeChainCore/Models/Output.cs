namespace VeChainCore.Models
{
    public class Output
    {
        public string contractAddress { get; set; }
        public Event[] events { get; set; }
        public Transfer[] transfers { get; set; }
    }
}
