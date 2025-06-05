using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExistHelper.Shared
{
    public static class SerializationHelper
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonConverter() }
        };

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }
    }
}