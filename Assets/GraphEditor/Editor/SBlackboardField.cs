using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class SBlackboardField : BlackboardField
    {
        private SBlackboard m_Blackboard;
        public SBlackboard Blackboard => m_Blackboard ??= GetFirstAncestorOfType<SBlackboard>();

        private TextField m_TextField;

        private FieldData m_FieldData;
        public FieldData FieldData => m_FieldData;

        public SBlackboardField(FieldData fieldData) : base()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);

            m_TextField = this.Q<TextField>();
            var textInputElement = m_TextField.Q(TextField.textInputUssName);
            textInputElement.RegisterCallback<FocusOutEvent>(e => { RequestFieldRename(); });
            m_FieldData = fieldData;
        }

        protected override void BuildFieldContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Rename", delegate
            {
                OpenTextEditor();
            }, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete", delegate
            {
                RequestFieldDelete();
            }, DropdownMenuAction.AlwaysEnabled);

        }

        private void RequestFieldRename()
        {
            this.Blackboard.RenameItem(this);
        }

        private void RequestFieldDelete()
        {
            this.Blackboard.contentContainer.Remove(this);
            Blackboard.RemoveItem(this);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (Event.current.type == EventType.MouseDrag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData("DragSelection", this);
                DragAndDrop.StartDrag("DragSelection");
                evt.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            evt.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
        }

        private void OnDragUpdated(DragUpdatedEvent e)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        }

        // https://discussions.unity.com/t/drags-can-only-be-started-from-mousedown-or-mousedrag-events/949156
        // https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-DragAndDrop.html
        // https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-Drag-Events.html
    }
}

