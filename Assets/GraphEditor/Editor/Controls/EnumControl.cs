using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class EnumControlView : VisualElement
    {
        private readonly BaseNodeView m_Node;
        private readonly PropertyInfo m_PropertyInfo;

        public EnumControlView(string label, BaseNodeView node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/EnumControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            if (!propertyInfo.PropertyType.IsEnum)
            {
                throw new ArgumentException("Property must be an enum.", "propertyInfo");
            }
            Add(new Label(label));
            var enumField = new EnumField((Enum)m_PropertyInfo.GetValue(m_Node, null));
            enumField.RegisterValueChangedCallback(OnValueChanged);
            Add(enumField);
        }

        private void OnValueChanged(ChangeEvent<Enum> evt)
        {
            var value = (Enum)m_PropertyInfo.GetValue(m_Node, null);
            if (!evt.newValue.Equals(value))
            {
                m_PropertyInfo.SetValue(m_Node, evt.newValue, null);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EnumControlAttribute : Attribute, IControlAttribute
    {
        private readonly string m_Label;

        public EnumControlAttribute(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(BaseNodeView node, PropertyInfo propertyInfo)
        {
            return new EnumControlView(m_Label, node, propertyInfo);
        }
    }
}