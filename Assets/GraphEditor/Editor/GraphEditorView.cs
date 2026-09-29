using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class GraphEditorView : VisualElement, IDisposable
    {
        private UniversalGraphView m_GraphView;
        private readonly GraphData m_GraphData;
        private readonly GraphWindow m_EditorWindow;
        private SearchWindowProvider m_provider;
        private SBlackboard m_Blackboard;
        private readonly BlackboardData m_BlackboardData;

        public Action SaveRequested { get; set; }
        
        public UniversalGraphView GraphView
        {
            get { return m_GraphView; }
        }

        public GraphEditorView(GraphWindow editorWindow, GraphData graph)
        {
            m_EditorWindow = editorWindow;
            m_GraphData = graph;

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/GraphEditorView"));
            Toolbar bar = new()
            {
                name = "toolbar"
            };
            Button saveBtn = new()
            {
                name = "button",
                iconImage = (Background)Resources.Load<Texture>("Icons/SaveActive"),
            };
            saveBtn.clickable.clicked += () => SaveRequested?.Invoke();
            bar.Add(saveBtn);
            Add(bar);

            var content = new VisualElement { name = "content" };
            {
                m_GraphView = new UniversalGraphView(m_GraphData)
                { name = "GraphView", viewDataKey = "UniversalGraphView" };
                ResetZoom();
                m_GraphView.AddManipulator(new ContentDragger());
                m_GraphView.AddManipulator(new SelectionDragger());
                m_GraphView.AddManipulator(new RectangleSelector());
                m_GraphView.AddManipulator(new ClickSelector());
                content.Add(m_GraphView);
            }
            Add(content);

            m_Blackboard = new SBlackboard()
            {
                title = "Variables",
                BlackboardTitle = "Variables",
                GraphEditorView = this
            };
            m_GraphView.Blackboard = m_Blackboard;
            m_BlackboardData = m_Blackboard.BlackboardData;
            m_GraphView.Add(m_Blackboard);

            m_provider = ScriptableObject.CreateInstance<SearchWindowProvider>();

            m_provider.OnSelectEntryHandler = OnMenuSelectEntry;
            m_GraphView.nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_provider);
            };

            AddNodes(graph.Nodes);
            if (!graph.Nodes.OfType<EntryNode>().Any())
            {
                EntryNode entry = Activator.CreateInstance(typeof(EntryNode)) as EntryNode;
                schedule.Execute(() =>
                {
                    float width = m_EditorWindow.position.width;
                    float height = m_EditorWindow.position.height;
                    Vector2 position = m_EditorWindow.ScreenPosition;
                    Vector2 centerToWindow = new(position.x + width / 2, position.y + height / 2);
                    Vector2 centerToView = PositionWindowToView(m_GraphView, PositionScreenToWindow(m_EditorWindow, centerToWindow));
                    AddNode(entry, new Vector2(centerToView.x - 300, centerToView.y));
                }).StartingIn(0);
                m_GraphData.AddNode(entry);
            }
            if (!graph.Nodes.OfType<OutputNode>().Any())
            {
                OutputNode output = Activator.CreateInstance(typeof(OutputNode)) as OutputNode;
                schedule.Execute(() =>
                {
                    float width = m_EditorWindow.position.width;
                    float height = m_EditorWindow.position.height;
                    Vector2 position = m_EditorWindow.ScreenPosition;
                    Vector2 centerToWindow = new(position.x + width / 2, position.y + height / 2);
                    Vector2 centerToView = PositionWindowToView(m_GraphView, PositionScreenToWindow(m_EditorWindow, centerToWindow));
                    AddNode(output, new Vector2(centerToView.x + 300, centerToView.y));
                }).StartingIn(0);
                m_GraphData.AddNode(output);
            }
            AddEdges(graph.Nodes);
            m_GraphView.CheckError();
        }

        void ResetZoom()
        {
            var weightedStepSize = WeightStepSize(EditorPrefs.GetFloat("UnityEditor.ShaderGraph.ZoomStepSize", 0.5f));
            m_GraphView?.SetupZoom(0.05f, 8.0f, weightedStepSize, 1.0f);
        }

        private static float WeightStepSize(float x) => Mathf.Clamp(2 * Mathf.Pow(x, 7f / 2f), 0.001f, 2.0f);


        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var clickPosition = context.screenMousePosition;
            Vector2 clickPositionRelatedToWindow = PositionScreenToWindow(m_EditorWindow, clickPosition);
            Vector2 clickPositionRelatedToGraphView = PositionWindowToView(m_GraphView, clickPositionRelatedToWindow);
            Vector2 expectPosition = clickPositionRelatedToGraphView;
            NodeData node = Activator.CreateInstance(searchTreeEntry.userData as Type) as NodeData;
            AddNode(node, expectPosition);
            m_GraphData.AddNode(node);
            m_GraphData.Owner.RegisterCompleteObjectUndo("Add " + node.Name);
            m_GraphView.CheckError();
            return true;
        }

        public static Vector2 PositionScreenToWindow(GraphWindow window, Vector2 screenPos)
        {
            return screenPos - window.ScreenPosition;
        }
        public static Vector2 PositionWindowToView(UniversalGraphView graphView, Vector2 windowPos)
        {
            var pos = windowPos - new Vector2(graphView.viewTransform.position.x, graphView.viewTransform.position.y);
            return pos / graphView.viewTransform.scale;
        }

        private void AddProperty(PropertyData property, Vector2 position)
        {
            PropertyNodeView propertyView = new(property);
            m_GraphView.AddElement(propertyView);
            propertyView.SetPosition(new Rect(position, propertyView.GetPosition().size));
            if (property.Guid == null)
            {
                property.Guid = propertyView.viewDataKey;
            }
            else
            {
                propertyView.viewDataKey = property.Guid;
            }
        }

        private void AddNode(NodeData node, Vector2 position)
        {
            Type type = GraphEditorUtils.NodeViews.ContainsKey(node.Name) ? GraphEditorUtils.NodeViews[node.Name] : typeof(BaseNodeView);
            BaseNodeView nodeView = (BaseNodeView)Activator.CreateInstance(type);
            m_GraphView.AddElement(nodeView);
            node.SetPosition(position);
            node.SetupSlots();
            if (node.Guid == null)
            {
                node.Guid = nodeView.viewDataKey;
            }
            else
            {
                nodeView.viewDataKey = node.Guid;
            }
            nodeView.Init(node);
        }

        private void AddNodes(IEnumerable<NodeData> nodes)
        {
            List<PropertyData> propertiesToAdd = new();

            List<PropertyData> propertiesToRemove = new();
            
            foreach (NodeData nodeData in nodes)
            {
                if (nodeData is PropertyData propertyData)
                {
                    FieldData fieldData = m_BlackboardData.FindDataFromAlter(propertyData.Guid);
                    if (fieldData != null)
                    {
                        if (propertyData.Name != fieldData.Name)
                        {
                            propertyData.Name = fieldData.Name;
                        }
                        propertiesToAdd.Add(propertyData);
                    }
                    else
                    {
                        propertiesToRemove.Add(propertyData);
                    }
                    continue;
                }
                AddNode(nodeData, nodeData.GetPosition());
            }

            foreach (PropertyData item in propertiesToRemove)
            {
                m_GraphData.RemoveNode(item);
            }

            foreach (PropertyData property in propertiesToAdd)
            {
                AddProperty(property, property.GetPosition());
            }
        }

        private void AddEdges(IEnumerable<NodeData> nodes)
        {
            foreach (var node in nodes)
            {
                for (int i = 0; i < node.Slots.Count; i++)
                {
                    if (node.Slots[i].Connections.Count == 0)
                    {
                        continue;
                    }
                    for (int j = 0; j < node.Slots[i].Connections.Count; j++)
                    {
                        Port output = m_GraphView.GetPortFromId(node.Slots[i].Guid);
                        Port input = m_GraphView.GetPortFromId(node.Slots[i].Connections[j]);
                        AddEdge(output, input);
                    }
                }
            }
        }

        private void AddEdge(Port output, Port input)
        {
            Edge edge = output is ActionPort ? output.ConnectTo<ActionEdge>(input) : output.ConnectTo<Edge>(input);
            m_GraphView.AddElement(edge);
            edge.UpdateEdgeControl();
        }

        public void HandleGraphChanges()
        {

        }

        public void Dispose()
        {
            if (m_GraphView != null)
            {
                SaveRequested = null;
                foreach (var nodeView in m_GraphView.Query<BaseNodeView>().ToList())
                {
                    nodeView.Dispose();
                }
                m_GraphView.nodeCreationRequest = null;
                m_GraphView.Dispose();
                m_GraphView = null;
            }
            m_Blackboard?.Dispose();
            m_Blackboard = null;
            Resources.UnloadAsset(PortView.styleSheet);
            if (m_provider != null)
            {
                // m_provider.Dispose();
                m_provider = null;
            }
        }
    }
}
