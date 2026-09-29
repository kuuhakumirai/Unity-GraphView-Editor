using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class StringInputNodeView : InputView
    {
        private TextField textField;
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
                    textField = new TextField()
                    {
                        multiline = true,
                    };
                    m_Control.Add(textField);
                }
                m_Container.Add(m_Control);
            }
            Add(m_Container);

            VisualElement border = this.Q<VisualElement>("node-border");
            Remove(border);

            AddPorts(node.Slots);

            m_Slot = node.Slots.FirstOrDefault(x => x.Name == "V");

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

