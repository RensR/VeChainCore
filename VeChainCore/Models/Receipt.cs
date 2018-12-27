using VeChainCore.Models.Meta;

namespace VeChainCore.Models
{
    public class Receipt
    {
        public  uint gasUsed { get; set; }
        public string gasPayer { get; set; }
        public string paid { get; set; }
        public string reward { get; set; }
        public bool reverted { get; set; }
        public Output[] outputs { get; set; }
        public LogMeta meta { get; set; }

    }
}
