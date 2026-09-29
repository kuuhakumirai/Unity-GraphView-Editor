using System.Linq;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class TextNodeView : BasicNodeView
    {
        private readonly TextField textField;
        private SlotData m_Slot;

        public TextNodeView() : base()
        {
            textField = new()
            {
                multiline = true,
            };
            contentContainer.Add(textField);
        }

        public override void Init(NodeData data)
        {
            base.Init(data);

            m_Slot = data.Slots.FirstOrDefault(x => x.Name == "V");

            textField.RegisterValueChangedCallback(OnValueChanged);

            if (!string.IsNullOrEmpty(m_Slot.Value))
            {
                textField.SetValueWithoutNotify(m_Slot.Value);
            }
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue != m_Slot.Value)
            {
                m_Slot.Value = evt.newValue;
                m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
            }
        }
    }
}
