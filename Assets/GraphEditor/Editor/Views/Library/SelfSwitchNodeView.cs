using System;
using System.Linq;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class SelfSwitchNodeView : BaseNodeView
    {
        private SelfSwitchCase m_Case;
        private SlotData m_Slot;

        [EnumControl("Case")]
        public SelfSwitchCase Case
        {
            get => m_Case;
            set
            {
                if (m_Case != value)
                {
                    m_Slot.Value = ((int)value).ToString();
                    m_Case = value;
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change" + node.Name);
                }
            }
        }

        public SelfSwitchNodeView() : base()
        {
        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();
            EnumField enumField = m_ControlItems.Q<EnumField>();
            if (Enum.TryParse(m_Slot.Value, out SelfSwitchCase result))
            {
                enumField.value = result;
            }
        }
    }
}
