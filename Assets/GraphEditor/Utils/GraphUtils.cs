using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphEditor
{
    public static class GraphUtils
    {
        public static T Deserialize<T>(string json)
        {
            JsonSerializerOptions options = new()
            {
                Converters =
            {
                new JsonStringEnumConverter()
            }
            };
            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static string Serialize<T>(T t)
        {
            JsonSerializerOptions options = new()
            {
                Converters =
            {
                new JsonStringEnumConverter()
            }
            };
            return JsonSerializer.Serialize(t, options);
        }
    }
}
