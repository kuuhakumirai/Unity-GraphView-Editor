using UnityEngine;

namespace GraphEditor
{
    public class GraphObject : ScriptableObject, ISerializable
    {
        [SerializeField]
        [HideInInspector]
        private string m_SerializedGraph;

        public GraphData Graph
        {
            get => m_Graph;
            set
            {
                m_Graph = value;
                if (m_Graph != null)
                {
                    m_Graph.Owner = this;
                }
            }
        }

        private GraphData m_Graph;

        [SerializeField]
        [HideInInspector]
        private bool m_IsDirty;

        public bool IsDirty
        {
            get { return m_IsDirty; }
            set { m_IsDirty = value; }
        }

        public void Deserialize()
        {
            if (!string.IsNullOrEmpty(m_SerializedGraph))
            {
                Graph = GraphUtils.Deserialize<GraphData>(m_SerializedGraph);
            }
        }

        public void Serialize()
        {
            if (Graph != null)
            {
                var json = GraphUtils.Serialize(Graph);
                m_SerializedGraph = json;
            }
        }

        public virtual void RegisterCompleteObjectUndo(string actionName)
        {
            // Undo.RegisterCompleteObjectUndo(this, actionName);
            m_IsDirty = true;
        }
    }
}
