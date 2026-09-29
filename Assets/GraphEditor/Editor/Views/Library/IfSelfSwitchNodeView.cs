using System;
using System.Linq;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class IfSelfSwitchNodeView : ActionNodeView
    {
        private State m_State;
        private SlotData m_Slot;

        [EnumControl("State")]
        public State State
        {
            get => m_State;
            set
            {
                if (m_State != value)
                {
                    m_Slot.Value = ((int)value).ToString();
                    m_State = value;
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                }
            }
        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();
            EnumField enumField = m_ControlItems.Q<EnumField>();
            if (Enum.TryParse(m_Slot.Value, out State result))
            {
                enumField.value = result;
            }
            CheckError();
        }
    }
}
