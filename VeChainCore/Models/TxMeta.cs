﻿using System;
using System.Collections.Generic;

namespace VeChainCore
{
    public class TxMeta : IEquatable<TxMeta>
    {
        public string blockID { get; set; }
        public UInt32 blockNumber { get; set; }
        public UInt64 blockTimestamp { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TxMeta);
        }

        public bool Equals(TxMeta other)
        {
            return other != null &&
                   blockID == other.blockID &&
                   blockNumber == other.blockNumber &&
                   blockTimestamp == other.blockTimestamp;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(blockID, blockNumber, blockTimestamp);
        }

        public static bool operator ==(TxMeta meta1, TxMeta meta2)
        {
            return EqualityComparer<TxMeta>.Default.Equals(meta1, meta2);
        }

        public static bool operator !=(TxMeta meta1, TxMeta meta2)
        {
            return !(meta1 == meta2);
        }
    }
}
