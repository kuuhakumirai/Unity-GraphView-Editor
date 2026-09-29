using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GraphEditor
{
    [JsonDerivedType(typeof(FieldData), typeDiscriminator: nameof(FieldData))]
    public partial class FieldData : IData
    {
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        private List<string> m_Alter;

        [JsonPropertyName("alter")]
        public IReadOnlyList<string> Alter
        {
            get => m_Alter;
            init => m_Alter = value.ToList();
        }

        public FieldData()
        {
            m_Alter ??= new List<string>();
        }

        public void AddAlter(string alter)
        {
            if (!m_Alter.Contains(alter))
            {
                m_Alter.Add(alter);
            }
        }

        public void RemoveAlter(string alter)
        {
            if (m_Alter.Contains(alter))
            {
                m_Alter.Remove(alter);
            }
        }
    }
}

