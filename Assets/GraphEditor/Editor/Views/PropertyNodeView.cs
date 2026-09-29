using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class PropertyNodeView : TokenNode
    {
        public static StyleSheet styleSheet;
        public PropertyNodeView(PropertyData data) : base(null, BasicPort.Create<Edge>(data.Slots.FirstOrDefault(x => !x.IsInputSlot)))
        {
            if (styleSheet == null)
                styleSheet = Resources.Load<StyleSheet>("Styles/PropertyNodeView");
            styleSheets.Add(styleSheet);
            output.portName = data.Name;
            output.viewDataKey = data.Slots.FirstOrDefault(x => !x.IsInputSlot).Guid;
        }
    }

}
