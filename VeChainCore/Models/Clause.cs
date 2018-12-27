using System;
using System.Collections.Generic;

namespace VeChainCore
{
    public class Clause : IEquatable<Clause>
    {
        public string to { get; set; }
        public string value { get; set; }
        public string data { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Clause);
        }

        public bool Equals(Clause other)
        {
            return other != null &&
               to == other.to &&
               value == other.value &&
               data == other.data;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(to, value, data);
        }

        public static bool operator ==(Clause clause1, Clause clause2)
        {
            return EqualityComparer<Clause>.Default.Equals(clause1, clause2);
        }

        public static bool operator !=(Clause clause1, Clause clause2)
        {
            return !(clause1 == clause2);
        }
    }
}
