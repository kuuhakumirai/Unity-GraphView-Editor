using System.Text.Json.Serialization;

namespace GraphEditor
{
    public class PropertyData : NodeData
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
