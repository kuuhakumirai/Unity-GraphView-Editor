using System.Reflection;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public interface IControlAttribute
    {
        VisualElement InstantiateControl(BaseNodeView node, PropertyInfo propertyInfo);
    }
}

