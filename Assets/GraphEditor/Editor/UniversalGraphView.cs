using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class UniversalGraphView : GraphView, IDisposable
    {
        public GraphData GraphData { get; private set; }
        private readonly List<BaseNodeView> m_ErrorNodeView;
        private readonly VisualElement m_ErrorBadge;
        private SBlackboard m_Blackboard;
        public SBlackboard Blackboard
        {
            get => m_Blackboard;
            set => m_Blackboard = value;
        }

        public UniversalGraphView(GraphData data) : this()
        {
            GraphData = data;
            graphViewChanged += OnGraphViewChange;
            m_ErrorNodeView = new();
            m_ErrorBadge = IconBadge.CreateError("This Graph Has Errors.");
            m_ErrorBadge.style.position = Position.Absolute;
            m_ErrorBadge.style.right = 10;
            m_ErrorBadge.style.top = 10;
            m_ErrorBadge.style.display = DisplayStyle.None;
            Add(m_ErrorBadge);
            RegisterCallback<DragEnterEvent>(OnDragEnter);
            RegisterCallback<DragLeaveEvent>(OnDragLeave);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            RegisterCallback<DragExitedEvent>(OnDragExit);
        }


        public override EventPropagation DeleteSelection()
        {
            HashSet<GraphElement> hashSet = new();
            CollectDeletableGraphElements(selection.OfType<GraphElement>(), hashSet);
            HashSet<GraphElement> hashSet2 = new();
            foreach (Placemat item in hashSet.OfType<Placemat>().Where(p => p.Collapsed))
            {
                hashSet2.UnionWith(item.CollapsedElements);
                item.Collapsed = false;
            }

            List<SBlackboardField> fieldToDelete = hashSet.OfType<SBlackboardField>().ToList();
            foreach (var item in fieldToDelete)
            {
                item.Blackboard.RemoveItem(item);
            }
            DeleteElements(hashSet);

            selection.Clear();
            foreach (GraphElement item2 in hashSet2)
            {
                AddToSelection(item2);
            }

            return (hashSet.Count <= 0) ? EventPropagation.Continue : EventPropagation.Stop;
        }

        private void CollectDeletableGraphElements(IEnumerable<GraphElement> elements, HashSet<GraphElement> elementsToRemoveSet)
        {
            CollectElements(elements, elementsToRemoveSet, e => (e.capabilities & Capabilities.Deletable) == Capabilities.Deletable);
        }

        #region Drag and drop
        private void OnDragPerform(DragPerformEvent e)
        {
            Vector2 localPos = (e.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, e.localMousePosition);
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> selection)
            {
                if (selection.OfType<SBlackboardField>().Any())
                {
                    IEnumerable<SBlackboardField> fields = selection.OfType<SBlackboardField>();
                    foreach (SBlackboardField field in fields)
                    {
                        PropertyData propertyData = (PropertyData)Activator.CreateInstance(BlackboardUtils.FieldToAddProperty[field.FieldData.GetType()]);
                        propertyData.Guid = Guid.NewGuid().ToString();
                        propertyData.Name = field.FieldData.Name;
                        field.FieldData.AddAlter(propertyData.Guid);
                        field.Blackboard.OnBlackboardChanged.Invoke();
                        PropertyNodeView propertyNodeView = new(propertyData)
                        {
                            viewDataKey = propertyData.Guid,
                        };
                        AddElement(propertyNodeView);
                        propertyNodeView.SetPosition(new Rect(localPos, propertyNodeView.GetPosition().size));
                        propertyData.SetPosition(new Vector2((int)localPos.x, (int)localPos.y));
                        GraphData.AddNode(propertyData);
                    }
                }
            }
            else
            {
            }
        }

        private void OnDragExit(DragExitedEvent evt)
        {
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void OnDragLeave(DragLeaveEvent evt)
        {
        }

        private void OnDragEnter(DragEnterEvent evt)
        {
        }

        #endregion

        public UniversalGraphView() : base()
        {
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startSlot = startAnchor.GetSlot();
            if (startSlot == null)
            {
                return compatiblePorts;
            }
            foreach (var port in ports.ToList())
            {                
                if (startAnchor.node == port.node ||
                    startAnchor.direction == port.direction ||
                    startAnchor.portType != port.portType)
                {
                    continue;
                }

                var candidateSlot = port.GetSlot();
                if (!startSlot.IsCompatibleWith(candidateSlot))
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }

        private GraphViewChange OnGraphViewChange(GraphViewChange change)
        {
            //��ͼ���Ƴ�Ԫ��ʱ
            change.elementsToRemove?.ForEach(elem =>
            {
                OnElementRemove(elem);
            });
            //��ͼ�д�������ʱ
            change.edgesToCreate?.ForEach(edge =>
            {
                OnEdgeCreate(edge);
            });
            //��ͼ���ƶ�Ԫ��ʱ
            change.movedElements?.ForEach(element =>
            {
                OnElementMove(element);
            });
            CheckError();
            GraphData.Owner.RegisterCompleteObjectUndo("GraphView Change");
            return change;
        }

        public void CheckError()
        {
            schedule.Execute(() =>
            {
                m_ErrorNodeView.Clear();

                List<BaseNodeView> nodes = this.Query<BaseNodeView>().ToList();

                foreach (var nodeView in this.Query<BaseNodeView>().ToList())
                {
                    if (nodeView.CheckError())
                    {
                        m_ErrorNodeView.Add(nodeView);
                    }
                }

                bool hasEntryAndOutput = nodes.OfType<EntryNodeView>().Any() && nodes.OfType<OutputNodeView>().Any();
                GraphData.HasError = m_ErrorNodeView.Count > 0 || !hasEntryAndOutput;
                if (GraphData.HasError)
                {
                    m_ErrorBadge.style.display = DisplayStyle.Flex;
                }
                else
                {
                    m_ErrorBadge.style.display = DisplayStyle.None;
                }
            }).StartingIn(0);
        }

        private void OnElementMove(GraphElement element)
        {
            if (element is BaseNodeView nodeView)
            {
                NodeData node = GraphData.GetNodeFromId(nodeView.viewDataKey);
                node?.SetPosition(element.GetPosition().position);
            }
            if (element is PropertyNodeView propertyNodeView)
            {
                NodeData node = GraphData.GetNodeFromId(propertyNodeView.viewDataKey);
                node?.SetPosition(element.GetPosition().position);
            }
        }

        private void OnEdgeCreate(Edge edge)
        {
            NodeData node = GraphData.GetNodeFromId(edge.output.node.viewDataKey);
            SlotData slot = node.GetSlotFromId(edge.output.viewDataKey);
            slot.Connections.Add(edge.input.viewDataKey);
        }

        private void OnElementRemove(GraphElement element)
        {
            if (element is BaseNodeView nodeView)
            {
                NodeData node = GraphData.GetNodeFromId(nodeView.viewDataKey);
                if (GraphData.Nodes.Contains(node))
                {
                    GraphData.RemoveNode(node);
                }
            }
            if (element is PropertyNodeView propertyNodeView)
            {
                NodeData property = GraphData.GetNodeFromId(propertyNodeView.viewDataKey);
                if (GraphData.Nodes.Contains(property))
                {
                    GraphData.RemoveNode(property);
                }
                m_Blackboard?.RemoveAlter(propertyNodeView.viewDataKey);
            }
            if (element is Edge edge)
            {
                if (edge.output != null)
                {
                    NodeData node = GraphData.GetNodeFromId(edge.output.node.viewDataKey);
                    if (node != null)
                    {
                        SlotData slot = node.GetSlotFromId(edge.output.viewDataKey);
                        if (slot.Connections.Contains(edge.input.viewDataKey))
                        {
                            slot.Connections.Remove(edge.input.viewDataKey);
                        }
                    }
                }
            }
        }

        public Port GetPortFromId(string guid)
        {
            return ports.ToList().FirstOrDefault(x => x.viewDataKey == guid);
        }

        public void Dispose()
        {
            UnregisterCallback<DragEnterEvent>(OnDragEnter);
            UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            UnregisterCallback<DragPerformEvent>(OnDragPerform);
            UnregisterCallback<DragExitedEvent>(OnDragExit);
        }
    }
}
