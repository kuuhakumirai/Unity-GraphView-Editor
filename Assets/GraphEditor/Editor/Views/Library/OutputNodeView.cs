using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphEditor
{

    public class OutputNodeView : ActionNodeView
    {
        public OutputNodeView() : base()
        {

        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            CheckError();
        }

        public override bool CheckError()
        {
            Port input = inputContainer.Q<Port>();
            if (!input.connected)
            {
                IconBadge badge = IconBadge.CreateError("This Port doesn't have an invalid connection.");
                AddBadge(badge);
            }
            else
            {
                ClearBadge();
            }
            return HasError;
        }

    }
}