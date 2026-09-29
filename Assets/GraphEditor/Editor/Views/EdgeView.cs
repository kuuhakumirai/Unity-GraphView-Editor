using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class ActionEdge : Edge
    {
        public ActionEdge() : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/EdgeView"));
            this.AddToClassList("ActionEdge");
        }

    }
}
