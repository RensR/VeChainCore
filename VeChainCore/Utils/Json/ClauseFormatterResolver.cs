using Utf8Json;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Utils.Json
{
    public class ClauseFormatterResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new ClauseFormatterResolver();

        private ClauseFormatterResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
            => typeof(T) == typeof(Clause) ? ClauseFormatter.Instance as IJsonFormatter<T> : null;
    }
}