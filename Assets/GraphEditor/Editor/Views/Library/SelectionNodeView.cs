using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class SelectionNodeView : ActionNodeView
    {
        public SelectionNodeView() : base()
        {
        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            CheckError();
        }

        public override bool CheckError()
        {
            int errors = 0;

            if (outputContainer[0].Q<Port>().connected)
            {
                errors++;
            }
            for (int i = 1; i < 6; i++)
            {
                if (!inputContainer[i].Q<Port>().connected && outputContainer[i].Q<Port>().connected)
                {
                    errors++;
                }
                if (inputContainer[i].Q<Port>().connected && !outputContainer[i].Q<Port>().connected)
                {
                    errors++;
                }
            }
            if (errors > 0)
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
