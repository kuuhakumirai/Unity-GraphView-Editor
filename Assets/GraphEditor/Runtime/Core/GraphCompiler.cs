using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace GraphEditor.Core
{
    public class GraphCompiler
    {
        public bool IsGraphValidate { get; }

        public AddressablePreLoader AddressablePreLoader { get; }

        private readonly Dictionary<NodeData, List<SlotData>> m_Nodes = new();

        public ReadOnlyDictionary<NodeData, List<SlotData>> SortedNodes { get; }


        public bool HasSelections { get; }

        public GraphCompiler(GraphObject graphObject)
        {
            graphObject.Deserialize();
            GraphData graph = graphObject.Graph;
            IsGraphValidate = !graph.HasError;
            foreach (var node in graph.Nodes)
            {
                if (!m_Nodes.ContainsKey(node))
                {
                    m_Nodes.Add(node, new List<SlotData>());
                    foreach (var slot in node.Slots)
                    {
                        m_Nodes[node].Add(slot);
                    }
                }
            }
            SortedNodes = new(SortNodes(m_Nodes));

            var guids = m_Nodes
                .Where(kvp => kvp.Key != null && kvp.Key.Name == "Image")
                .SelectMany(kvp => kvp.Value)
                .Where(slot => slot != null && slot.Name == "V")
                .Select(x => x.Value);

            if (guids.Any())
            {
                AddressablePreLoader = new(guids);
            }

        }

        public KeyValuePair<NodeData, List<SlotData>> GetSourceFromInput(SlotData slot)
        {
            var matchingKvp = m_Nodes
                .Where(kvp => kvp.Value != null && kvp.Value.Any(s => s.Connections != null && s.Connections.Any(x => x == slot.Guid)))
                .FirstOrDefault();
            return matchingKvp;
        }
        public Dictionary<NodeData, List<SlotData>> SortNodes(Dictionary<NodeData, List<SlotData>> dic)
        {
            var result = new Dictionary<NodeData, List<SlotData>>();
            if (dic?.Any() != true) return result;

            var startNode = dic.First(kvp => kvp.Value.Any(slot => slot.Name == "Out"));
            result.Add(startNode.Key, startNode.Value);

            var processedNodes = new HashSet<NodeData> { startNode.Key };

            startNode.Value
                .Where(s => s.Name.Contains("Out"))
                .Select(s => s.Connections?.FirstOrDefault())
                .Where(g => !string.IsNullOrEmpty(g))
                .ToList()
                .ForEach(g => ProcessChain(dic, result, processedNodes, g));

            return result;
        }

        private void ProcessChain(
            Dictionary<NodeData, List<SlotData>> dic,
            Dictionary<NodeData, List<SlotData>> result,
            HashSet<NodeData> processedNodes,
            string directionGuid)
        {
            var nextNode = dic
                .Where(kvp => !processedNodes.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value
                    .Where(s => s.Name == "In" && s.Guid == directionGuid)
                    .Select(s => new { Node = kvp.Key, Slots = kvp.Value }))
                .FirstOrDefault();

            if (nextNode == null) return;

            if (!result.ContainsKey(nextNode.Node))
            {
                result.Add(nextNode.Node, nextNode.Slots);
                processedNodes.Add(nextNode.Node);
            }

            nextNode.Slots
                .Where(s => s.Name.Contains("Out"))
                .SelectMany(s => s.Connections ?? new List<string>())
                .Where(g => !string.IsNullOrEmpty(g))
                .ToList()
                .ForEach(g => ProcessChain(dic, result, processedNodes, g));
        }
    }
}

