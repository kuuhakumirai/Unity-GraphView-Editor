using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace GraphEditor
{
    public class SlotData
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("types")]
        public SlotType Types { get; init; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("connections")]
        public List<string> Connections
        {
            get => m_Connections;
            init => m_Connections = value;
        }

        [JsonIgnore]
        public NodeData Owner { get; set; }

        public readonly struct SlotType
        {
            [JsonPropertyName("direction")]
            public SlotDirection Direction { get; init; }

            [JsonPropertyName("feature")]
            public SlotFeatureType Feature { get; init; }

            [JsonPropertyName("type")]
            public SlotValueType ValueType { get; init; }

        }

        [SerializeField]
        private List<string> m_Connections;

        public SlotData()
        {
            m_Connections ??= new List<string>();
            Guid = System.Guid.NewGuid().ToString();
        }

        [JsonIgnore]
        public bool IsInputSlot => Types.Direction == SlotDirection.Input;

        [JsonIgnore]
        public bool IsActionSlot => Types.Feature == SlotFeatureType.Action;

        public bool IsCompatibleWith(SlotData other)
        {
            return other != null
                && Types.Feature == other.Types.Feature
                && Types.ValueType == other.Types.ValueType;
        }
    }
}