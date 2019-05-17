using System.Runtime.Serialization;

namespace VeChainCore.Models.Blockchain
{
    [DataContract]
    public class TransferCriteria
    {
        [DataMember]
        public string txOrigin { get; set; }
        [DataMember]
        public string sender { get; set; }
        [DataMember]
        public string recipient { get; set; }
    }
    [DataContract]
    public class EventCriteria
    {
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string topic0 { get; set; }
        [DataMember]
        public string topic1 { get; set; }
        [DataMember]
        public string topic2 { get; set; }
        [DataMember]
        public string topic3 { get; set; }
        [DataMember]
        public string topic4 { get; set; }
    }
}

/*
{
  "options": {
    "offset": 0,
    "limit": 10
  },
  "criteriaSet": [
    {
      "address": "0x9c6e62B3334294D70c8e410941f52D482557955B",
      "topic0":"0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef",
      "topic2": "0x00000000000000000000000069e38dac88c2db5568242ce381625920919e3bb3"
    }
  ],
  "order": "asc"
}
*/