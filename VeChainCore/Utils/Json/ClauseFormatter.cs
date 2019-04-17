using System.Text;
using Utf8Json;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Utils.Json
{
    public class ClauseFormatter : IJsonFormatter<Clause>
    {
        private ClauseFormatter()
        {
        }

        private static readonly byte[] PropertyNameTo = Encoding.UTF8.GetBytes("\"to\"");
        private static readonly byte[] PropertyNameValue = Encoding.UTF8.GetBytes("\"value\"");
        private static readonly byte[] PropertyNameData = Encoding.UTF8.GetBytes("\"data\"");

        public void Serialize(ref JsonWriter writer, Clause clause, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginObject();
            writer.WriteRaw(PropertyNameTo);
            writer.WriteNameSeparator();
            writer.WriteString(clause.to);
            writer.WriteValueSeparator();
            writer.WriteRaw(PropertyNameValue);
            writer.WriteNameSeparator();
            ((IJsonFormatter<decimal>) VeChainHexFormatter.Default)
                .Serialize(ref writer, clause.value, formatterResolver);
            writer.WriteValueSeparator();
            writer.WriteRaw(PropertyNameData);
            writer.WriteNameSeparator();
            writer.WriteString(clause.data);
            writer.WriteEndObject();
        }

        public Clause Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            => JsonSerializer.Deserialize<ArbitraryClause>(ref reader, formatterResolver);

        public static readonly IJsonFormatter<Clause> Instance = new ClauseFormatter();
    }
}