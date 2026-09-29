using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class IfVariableNodeView : ActionNodeView
    {
        private Condition m_Condition;
        private SlotData m_Slot;

        [EnumControl("Condition")]
        public Condition Condition
        {
            get => m_Condition;
            set
            {
                if (m_Condition != value)
                {
                    m_Slot.Value = ((int)value).ToString();
                    m_Condition = value;
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                }
            }
        }

        public IfVariableNodeView() : base()
        {

        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            EnumField enumField = m_ControlItems.Q<EnumField>();
            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();
            if (Enum.TryParse(m_Slot.Value, out Condition result))
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

