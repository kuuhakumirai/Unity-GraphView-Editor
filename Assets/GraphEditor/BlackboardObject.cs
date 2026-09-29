using System.Text.Json;
using UnityEngine;

namespace GraphEditor
{
    public class BlackboardObject : ScriptableObject, ISerializable
    {
        [SerializeField]
        [HideInInspector]
        private string m_SerializedBlackboard;

        public BlackboardData Blackboard
        {
            get => m_Blackboard;
            set => m_Blackboard = value;
        }

        private BlackboardData m_Blackboard;

        public void Deserialize()
        {
            if (string.IsNullOrWhiteSpace(m_SerializedBlackboard))
            {
                Blackboard ??= new();
                return;
            }
            Blackboard = JsonSerializer.Deserialize<BlackboardData>(m_SerializedBlackboard) ?? new();
        }

        public void Serialize()
        {
            if (Blackboard != null)
            {
                m_SerializedBlackboard = JsonSerializer.Serialize(Blackboard);
            }     
        }
    }
}
