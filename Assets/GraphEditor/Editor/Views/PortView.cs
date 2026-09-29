using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace GraphEditor
{
    public class PortView : Port
    {
        public static StyleSheet styleSheet;

        protected PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
            if (styleSheet == null)
            {
                styleSheet = Resources.Load<StyleSheet>("Styles/PortView");
            }
            styleSheets.Add(styleSheet);
        }

        private SlotData m_Port;
        
        public SlotData Port
        {
            get => m_Port;
            set
            {
                if (ReferenceEquals(value, m_Port))
                {
                    return;
                }
                if (value == null)
                {
                    return;
                }
                m_Port = value; 
            }
        }

        protected class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;

            private readonly List<Edge> m_EdgesToCreate;

            private readonly List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                        {
                            m_EdgesToDelete.Add(connection);
                        }
                    }
                }

                if (edge.output.capacity == Capacity.Single)
                {
                    foreach (Edge connection2 in edge.output.connections)
                    {
                        if (connection2 != edge)
                        {
                            m_EdgesToDelete.Add(connection2);
                        }
                    }
                }

                if (m_EdgesToDelete.Count > 0)
                {
                    graphView.DeleteElements(m_EdgesToDelete);
                }

                List<Edge> edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge item in edgesToCreate)
                {
                    graphView.AddElement(item);
                    edge.input.Connect(item);
                    edge.output.Connect(item);
                }
            }
        }
    }

    static class PortViewExtensions
    {
        public static SlotData GetSlot(this Port port)
        {
            return port is PortView portView ? portView.Port : null;
        }
    }

    public class ActionPort : PortView
    {
        protected ActionPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static ActionPort Create<TEdge>(SlotData data) where TEdge : Edge, new()
        {
            DefaultEdgeConnectorListener listener = new();
            ActionPort port = new(Orientation.Horizontal, data.IsInputSlot ? Direction.Input : Direction.Output, data.IsInputSlot ? Capacity.Multi : Capacity.Single, typeof(ActionPort))
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.Port = data;
            port.visualClass = "type_Action";
            return port;
        }
    }

    public class BasicPort : PortView
    {
        protected BasicPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static BasicPort Create<TEdge>(SlotData data) where TEdge : Edge, new()
        {
            DefaultEdgeConnectorListener listener = new();
            BasicPort port = new(Orientation.Horizontal, data.IsInputSlot ? Direction.Input : Direction.Output, data.IsInputSlot ? Capacity.Single : Capacity.Multi, typeof(BasicPort))
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.Port = data;
            port.visualClass = data.Types.ValueType.ToClassName();
            return port;
        }
    }
}
