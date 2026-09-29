using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class BaseNodeView : Node, IDisposable
    {
        public Action<BaseNodeView, NodeData> OnNodeViewUpdated;

        protected NodeData node;

        protected bool HasError => badges.Count > 0;
        protected List<IconBadge> badges = new();
        protected virtual bool HasPreview => false;

        protected VisualElement m_PreviewContainer;
        protected Image m_PreviewImage;
        protected VisualElement m_PreviewFiller;
        protected VisualElement m_ControlItems;
        protected VisualElement m_ControlsDivider;

        protected PreviewData m_PreviewData;

        public virtual bool CheckError()
        {
            return HasError;
        }

        protected void AddBadge(IconBadge badge)
        {
            if (badges.Count == 0)
            {
                Add(badge);
                badges.Add(badge);
                badge.AttachTo(topContainer, SpriteAlignment.TopRight);
                UpdateErrorStyle();
            }
        }

        protected void ClearBadge()
        {
            var badge = this.Q<IconBadge>();
            if (badge != null)
            {
                badge.Detach();
                badge.RemoveFromHierarchy();
                badges.Remove(badge);
            }
            UpdateErrorStyle();
        }

        private void UpdateErrorStyle()
        {
            if (HasError)
            {
                AddToClassList("ErrorNode");
            }
            else
            {
                RemoveFromClassList("ErrorNode");
            }
        }

        public BaseNodeView()
        {

        }

        public virtual void OnNodeViewUpdate(BaseNodeView node, NodeData nodea)
        {

        }

        public virtual void Init(NodeData node)
        {
            this.node = node;
            this.SetPosition(new Rect(node.GetPosition(), this.GetPosition().size));
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NodeView"));
            this.title = node.Name;
            var contents = this.Q("contents");

            var controlsContainer = new VisualElement { name = "controls" };
            {
                m_ControlsDivider = new VisualElement { name = "divider" };
                m_ControlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(m_ControlsDivider);
                m_ControlItems = new VisualElement { name = "items" };
                controlsContainer.Add(m_ControlItems);

                // Instantiate control views from node
                foreach (var propertyInfo in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    foreach (IControlAttribute attribute in propertyInfo.GetCustomAttributes(typeof(IControlAttribute), false).Cast<IControlAttribute>())
                    {
                        m_ControlItems.Add(attribute.InstantiateControl(this, propertyInfo));
                    }
                }
            }
            if (m_ControlItems.childCount > 0)
            {
                contents.Add(controlsContainer);
            }

            if (HasPreview)
            {
                m_PreviewData = new()
                {
                    PreviewName = node.Name ?? "UNNAMED NODE",
                };
                m_PreviewContainer = new VisualElement
                {
                    name = "previewContainer",
                    style = { overflow = Overflow.Hidden },
                    pickingMode = PickingMode.Ignore
                };
                m_PreviewImage = new Image
                {
                    name = "preview",
                    pickingMode = PickingMode.Ignore,
                    image = Texture2D.whiteTexture,
                };
                {
                    var collapsePreviewButton = new VisualElement { name = "collapse" };
                    collapsePreviewButton.Add(new VisualElement { name = "icon" });
                    collapsePreviewButton.AddManipulator(new Clickable(() =>
                    {
                        SetPreviewExpandedState(false);
                    }));
                    m_PreviewImage.Add(collapsePreviewButton);
                }
                m_PreviewContainer.Add(m_PreviewImage);
                m_PreviewData.OnPreviewChanged += UpdatePreview;
                m_PreviewFiller = new VisualElement { name = "previewFiller" };
                m_PreviewFiller.AddToClassList("expanded");
                {
                    var previewDivider = new VisualElement { name = "divider" };
                    previewDivider.AddToClassList("horizontal");
                    m_PreviewFiller.Add(previewDivider);

                    var expandPreviewButton = new VisualElement { name = "expand" };
                    expandPreviewButton.Add(new VisualElement { name = "icon" });
                    expandPreviewButton.AddManipulator(new Clickable(() =>
                    {
                        SetPreviewExpandedState(true);
                    }));
                    m_PreviewFiller.Add(expandPreviewButton);
                }
                contents.Add(m_PreviewFiller);
                UpdatePreviewExpandedState(node.PreviewExpanded);
            }
            AddPorts(node.Slots);
            UpdateInputView();
            RefreshPorts();
        }

        void UpdatePreviewExpandedState(bool expanded)
        {
            node.PreviewExpanded = expanded;
            if (m_PreviewFiller == null)
                return;
            if (expanded)
            {
                if (m_PreviewContainer.parent != this)
                {
                    Add(m_PreviewContainer);
                    m_PreviewContainer.PlaceBehind(this.Q("selection-border"));
                }
                m_PreviewFiller.AddToClassList("expanded");
                m_PreviewFiller.RemoveFromClassList("collapsed");
                m_PreviewImage.RemoveFromClassList("Hidden");
            }
            else
            {
                if (m_PreviewContainer.parent == m_PreviewFiller)
                {
                    m_PreviewContainer.RemoveFromHierarchy();
                }
                m_PreviewFiller.RemoveFromClassList("expanded");
                m_PreviewFiller.AddToClassList("collapsed");
                m_PreviewImage.AddToClassList("Hidden");
            }
        }

        protected virtual void UpdatePreview()
        {

        }

        protected virtual void SetPreviewExpandedState(bool state)
        {
            node.PreviewExpanded = state;
            node.Owner.Owner.RegisterCompleteObjectUndo(state ? "Expand Previews" : "Collapse Previews");
            UpdatePreviewExpandedState(node.PreviewExpanded);
        }

        private void UpdateInputView()
        {
            bool isInputContainerVisible = false;
            foreach (var item in inputContainer.Children())
            {
                if (!item.GetClasses().Contains("Hidden"))
                {
                    isInputContainerVisible = true;
                }
            }
            if (!isInputContainerVisible)
            {
                inputContainer.style.visibility = Visibility.Hidden;
                inputContainer.style.display = DisplayStyle.None;
            }
        }

        protected virtual void AddPorts(IEnumerable<SlotData> slots)
        {
            foreach (var slot in slots)
            {
                Port port = AddPort(slot);
                bool isVisible = slot.Name != "V";
                UpdatePortVisibility(port, isVisible);
            }
        }

        protected virtual Port AddPort(SlotData slot)
        {
            Port port = CreatePort(slot);
            port.portName = slot.Name;
            port.name = slot.Name;
            if (slot.Guid != null)
            {
                port.viewDataKey = slot.Guid;
            }
            else
            {
                slot.Guid = port.viewDataKey;
            }
            return port;
        }

        private void UpdatePortVisibility(Port port, bool isVisible)
        {
            SetElementVisible(port, isVisible);
        }

        private void SetElementVisible(VisualElement element, bool isVisible)
        {
            const string k_HiddenClassList = "Hidden";
            
            if (isVisible)
            {
                element.style.visibility = StyleKeyword.Null;
                element.RemoveFromClassList(k_HiddenClassList);
            }
            else
            {
                element.style.visibility = Visibility.Hidden;
                element.AddToClassList(k_HiddenClassList);
            }
        }

        protected virtual Port CreatePort(SlotData slot)
        {
            SlotDirection direction = slot.Types.Direction;
            var container = direction == SlotDirection.Input ? inputContainer : outputContainer;
            Port port = slot.Types.Feature == 
                SlotFeatureType.Action ? ActionPort.Create<ActionEdge>(slot)
                : BasicPort.Create<Edge>(slot);
            container.Add(port);
            return port;
        }

        public virtual void Dispose()
        {
            if (m_PreviewData != null)
            {
                m_PreviewData.Dispose();
                m_PreviewData.OnPreviewChanged -= UpdatePreview;
                m_PreviewData = null;
            }
            styleSheets.Clear();
            inputContainer?.Clear();
            outputContainer?.Clear();
            m_PreviewContainer?.Clear();
            badges.Clear();
        }
    }

    public class ActionNodeView : BaseNodeView
    {
        protected ActionNodeView() : base()
        {

        }
    }

    public class BasicNodeView : BaseNodeView
    {
        protected BasicNodeView() : base()
        {

        }
    }

}

