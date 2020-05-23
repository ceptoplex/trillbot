using System;
using Newtonsoft.Json;

namespace TrillBot.Common.Converters
{
    public sealed class SecondsToTimeSpanConverter : JsonConverter<TimeSpan?>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan? value, JsonSerializer serializer)
        {
            if (value.HasValue)
                if (value.Value.Milliseconds == 0)
                    writer.WriteValue((long) Math.Floor(value.Value.TotalSeconds));
                else
                    writer.WriteValue(value.Value.TotalSeconds);
            else
                writer.WriteNull();
        }

        public override TimeSpan? ReadJson(
            JsonReader reader,
            Type objectType,
            TimeSpan? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return reader.TokenType switch
            {
                JsonToken.Integer => TimeSpan.FromSeconds((long) reader.Value),
                JsonToken.Float => TimeSpan.FromSeconds((double) reader.Value),
                JsonToken.Null => null,
                _ => throw new JsonSerializationException()
            };
        }
    }
}