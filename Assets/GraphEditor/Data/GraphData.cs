using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GraphEditor
{
    public class GraphData
    {
        private List<NodeData> m_Nodes;

        [JsonPropertyName("hasError")]
        public bool HasError { get; set; }

        [JsonIgnore]
        public GraphObject Owner { get; set; }

        public void AddNode(NodeData node)
        {
            AddNodeNoValidate(node);
        }

        private void AddNodeNoValidate(NodeData node)
        {
            node.Owner = this;
            m_Nodes.Add(node);
            m_AddedNodes.Add(node);
        }

        public void RemoveNode(NodeData node)
        {
            RemoveNodeNoValidate(node);
        }

        private void RemoveNodeNoValidate(NodeData node)
        {
            m_Nodes.Remove(node);
            m_RemovedNodes.Add(node);
        }

        public GraphData()
        {
            m_Nodes ??= new List<NodeData>();
        }

        public NodeData GetNodeFromId(string nodeId)
        {
            NodeData node = m_Nodes.FirstOrDefault(x => x.Guid == nodeId);
            return node;
        }

        [JsonPropertyName("nodes")]
        public IReadOnlyList<NodeData> Nodes
        {
            get => m_Nodes;
            init => m_Nodes = value.ToList();
        }

        [NonSerialized]
        private List<NodeData> m_AddedNodes = new();

        [JsonIgnore]
        public IEnumerable<NodeData> AddedNodes => m_AddedNodes;

        [NonSerialized]
        private List<NodeData> m_RemovedNodes = new();

        [JsonIgnore]
        public IEnumerable<NodeData> RemovedNodes => m_RemovedNodes;

        public void ClearChanges()
        {
            m_AddedNodes.Clear();
            m_RemovedNodes.Clear();
        }
    }
}

