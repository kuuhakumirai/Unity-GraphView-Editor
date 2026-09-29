using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class FloatInputNodeView : InputView
    {
        private FloatField floatField;
        private SlotData m_Slot;

        public override void Init(NodeData node)
        {
            this.node = node;
            this.SetPosition(new Rect(node.GetPosition(), this.GetPosition().size));

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/InputView"));

            m_Container = new VisualElement { name = "container" };
            {
                m_Control = new VisualElement { name = "control" };
                {
                    floatField = new FloatField();
                    m_Control.Add(floatField);
                }
                m_Container.Add(m_Control);
            }
            Add(m_Container);

            VisualElement border = this.Q<VisualElement>("node-border");
            Remove(border);

            AddPorts(node.Slots);

            m_Slot = node.Slots.FirstOrDefault(x => x.Name == "V");

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
            float oldValue = evt.previousValue;
            if (SetValueValidated(evt.newValue.ToString(), out string newValue))
            {
                if (newValue != m_Slot.Value)
                {
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                    m_Slot.Value = newValue;
                }
            }
            else
            {
                floatField.SetValueWithoutNotify(oldValue);
            }
        }

        private bool SetValueValidated(string str, out string value)
        {
            bool isValidate = false;
            if (float.TryParse(str, out float newValue))
            {
                value = newValue.ToString();
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

