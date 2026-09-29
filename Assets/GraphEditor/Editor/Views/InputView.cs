using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace GraphEditor
{
    public class InputView : BaseNodeView
    {
        protected VisualElement m_Control;
        protected VisualElement m_Container;

        protected override Port AddPort(SlotData slot)
        {
            Port port = CreatePort(slot);
            port.portName = string.Empty;
            if (slot.Guid != null)
            {
                port.viewDataKey = slot.Guid;
            }
            else
            {
                slot.Guid = port.viewDataKey;
            }
            return port;
        }


        protected override Port CreatePort(SlotData slot)
        {
            Port port = slot.Types.Feature ==
                SlotFeatureType.Action ? ActionPort.Create<ActionEdge>(slot)
                : BasicPort.Create<Edge>(slot);
            port.name = "slot";
            m_Container.Add(port);
            return port;
        }
    }
}