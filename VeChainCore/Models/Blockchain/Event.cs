using System;
using System.Collections.Generic;
using System.Text;

namespace VeChainCore.Models
{
    public class Event
    {
        public string address { get; set; }
        public string[] topics { get; set; }
        public string data { get; set; }
    }
}
