using System;
using System.Collections.Generic;
using System.Text;

namespace VeChainCore.Models.Core
{
    class RawClause
    {
        public Address To { get; set; }
        public IAmount Value { get; set; }
        public string Data { get; set; }

    }
}
