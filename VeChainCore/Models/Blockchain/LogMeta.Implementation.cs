using System;
using System.Collections.Generic;
using Utf8Json;
using VeChainCore.Client;

namespace VeChainCore.Models.Blockchain
{
    public partial class LogMeta
    {
        public override string ToString()
        {
            return JsonSerializer.ToJsonString(this, VeChainClient.JsonFormatterResolver);
        }

        public override bool Equals(object obj)
        {
            return Equals((LogMeta) obj);
        }

        public bool Equals(LogMeta other)
        {
            return other != null &&
                   base.Equals(other) &&
                   txID == other.txID &&
                   txOrigin == other.txOrigin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), txID, txOrigin);
        }

        public static bool operator ==(LogMeta meta1, LogMeta meta2)
        {
            return EqualityComparer<LogMeta>.Default.Equals(meta1, meta2);
        }

        public static bool operator !=(LogMeta meta1, LogMeta meta2)
        {
            return !(meta1 == meta2);
        }
    }
}