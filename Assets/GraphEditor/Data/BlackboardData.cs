using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GraphEditor
{
    public class BlackboardData
    {
        [JsonPropertyName("fields")]
        public IReadOnlyList<FieldData> Fields
        {
            get => m_Fields;
            init => m_Fields = value?.ToList() ?? new List<FieldData>();
        }

        public BlackboardData()
        {
            m_Fields ??= new List<FieldData>();
        }

        private List<FieldData> m_Fields;

        public void AddField(FieldData fieldData)
        {
            m_Fields.Add(fieldData);
        }

        public void RemoveField(FieldData fieldData)
        {
            if (m_Fields.Contains(fieldData))
            {
                m_Fields.Remove(fieldData);
            }
        }

        public FieldData FindDataFromId(string id)
        {
            return m_Fields.FirstOrDefault(x => x.Guid == id);
        }

        public FieldData FindDataFromAlter(string alter)
        {
            foreach (var item in m_Fields)
            {
                if (item.Alter.Contains(alter))
                {
                    return item;
                }
            }
            return null;
        }

        public void RenameField(FieldData fieldData, string name)
        {
            fieldData.Name = name;
        }

    }

}
