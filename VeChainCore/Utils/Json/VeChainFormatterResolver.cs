using Utf8Json;

namespace VeChainCore.Utils.Json
{
    public class VeChainFormatterResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new VeChainFormatterResolver();

        private VeChainFormatterResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
            => VeChainHexFormatter.Default as IJsonFormatter<T>;
    }
}