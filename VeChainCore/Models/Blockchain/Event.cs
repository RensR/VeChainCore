namespace VeChainCore.Models.Blockchain
{
    public class Event
    {
        public string address { get; set; }
        public string[] topics { get; set; }
        public string data { get; set; }
    }
}
