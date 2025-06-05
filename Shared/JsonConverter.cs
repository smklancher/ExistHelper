using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExistHelper.Shared
{
    // Handles conversion of JSON types to C# types (string, int, double, bool, etc.)
    // for now leaving the copilot version.  Originally found the concept here: https://stackoverflow.com/a/65974452/221018
    // and comments also pointed to a documented version here:
    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-6-0#deserialize-inferred-types-to-object-properties
    public class JsonConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long l))
                        return l;
                    if (reader.TryGetDouble(out double d))
                        return d;
                    return reader.GetDecimal();
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return reader.GetBoolean();
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.StartObject:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                        return doc.RootElement.Clone();
                case JsonTokenType.StartArray:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                        return doc.RootElement.Clone();
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object), options);
        }
    }
}