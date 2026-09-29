using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class EntryNodeView : ActionNodeView
    {
        public EntryNodeView() : base()
        {
        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            CheckError();
        }

        public override bool CheckError()
        {
            Port output = outputContainer.Q<Port>();
            if (!output.connected)
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
