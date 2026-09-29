using System.Linq;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class IntNodeView : BaseNodeView
    {
        private readonly FloatField intField;
        private SlotData m_Slot;
        public IntNodeView() : base()
        {
            intField = new();
            contentContainer.Add(intField);
        }

        public override void Init(NodeData data)
        {
            base.Init(data);
            m_Slot = data.Slots.FirstOrDefault(x => x.Name == "V");

            intField.RegisterValueChangedCallback(OnValueChanged);

            intField.SetValueWithoutNotify(0);

            if (!string.IsNullOrEmpty(m_Slot.Value))
            {
                string str = m_Slot.Value;

                if (float.TryParse(str, out float value))
                {
                    intField.SetValueWithoutNotify(value);
                }
            }
        }

        private void OnValueChanged(ChangeEvent<float> evt)
        {
            float oldValue = evt.previousValue;
            if (SetValueValidated(evt.newValue.ToString(), out string newValue))
            {
                if (newValue != m_Slot.Value)
                {
                    m_Slot.Value = newValue;
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                    if (float.TryParse(newValue, out float value))
                    {
                        intField.SetValueWithoutNotify((int)value);
                    }
                }
            }
            else
            {
                intField.SetValueWithoutNotify(oldValue);
            }
        }

        private bool SetValueValidated(string str, out string value)
        {
            bool isValidate = false;
            if (float.TryParse(str, out float newValue))
            {
                value = ((int)newValue).ToString();
                isValidate = true;
            }
            else
            {
                value = str;
            }
            return isValidate;
        }
    }
}