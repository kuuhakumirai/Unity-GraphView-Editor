using System.Linq;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class FloatNodeView : BaseNodeView
    {
        private readonly FloatField floatField;
        private SlotData m_Slot;
        public FloatNodeView() : base()
        {
            floatField = new();
            contentContainer.Add(floatField);
        }

        public override void Init(NodeData data)
        {
            base.Init(data);

            m_Slot = data.Slots.FirstOrDefault(x => x.Name == "V");

            floatField.RegisterValueChangedCallback(OnValueChanged);

            floatField.SetValueWithoutNotify(0);

            if (!string.IsNullOrEmpty(m_Slot.Value))
            {
                string str = m_Slot.Value;

                if (float.TryParse(str, out float value))
                {
                    floatField.SetValueWithoutNotify(value);
                }
            }
        }
        
        private void OnValueChanged(ChangeEvent<float> evt)
        {
            if (evt.newValue.ToString() != m_Slot.Value)
            {
                m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                m_Slot.Value = evt.newValue.ToString();
            }
        }
    }

}
