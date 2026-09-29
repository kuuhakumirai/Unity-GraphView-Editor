using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class ChangePackageNodeView : ActionNodeView
    {
        private Operation m_Operation;
        private SlotData m_Slot;

        [EnumControl("Operation")]
        public Operation Operation
        {
            get => m_Operation;
            set
            {
                if (m_Operation != value)
                {
                    m_Slot.Value = ((int)value).ToString();
                    m_Operation = value;
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                }
            }
        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            EnumField enumField = m_ControlItems.Q<EnumField>();
            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();
            if (Enum.TryParse(m_Slot.Value, out Operation result))
            {
                enumField.value = result;
            }
            CheckError();
        }

        public override bool CheckError()
        {
            Port constantPort = inputContainer.Q<Port>("Variable");
            Port variablePort = inputContainer.Q<Port>("Constant");
            if (constantPort.connected && variablePort.connected)
            {
                IconBadge badge = IconBadge.CreateError("This Node has two operands connected.");
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