/*
using Core.SO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class ItemNodeView : BasicNodeView
    {
        protected override bool HasPreview => true;

        private ObjectField objectField;
        private SlotData m_Slot;
        private ItemSO m_PreviewItemSO;

        public override void Init(NodeData data)
        {
            base.Init(data);

            objectField = new ObjectField()
            {
                objectType = typeof(ItemSO),
            };
            contentContainer.Add(objectField);
            m_PreviewImage.AddToClassList("Hidden");
            m_PreviewFiller.AddToClassList("Hidden");

            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();

            objectField.RegisterValueChangedCallback(OnValueChanged);

            if (!string.IsNullOrEmpty(m_Slot.Value))
            {
                m_PreviewData.Content = GetItemSOFromGuid(m_Slot.Value);
                if (m_PreviewData.Content is ItemSO itemSO)
                {
                    m_PreviewItemSO = itemSO;
                    objectField.SetValueWithoutNotify(m_PreviewItemSO);
                    m_PreviewData.NotifyPreviewChanged();
                }
            }
        }

        private void OnValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            if (evt.newValue == null)
            {
                if (m_Slot.Value != null)
                {
                    m_Slot.Value = "";
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                    if (m_PreviewData != null)
                    {
                        m_PreviewData.Content = null;
                        m_PreviewData.NotifyPreviewChanged();
                    }
                }
            }
            else
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(evt.newValue, out string guid, out long _))
                {
                    if (guid != m_Slot.Value)
                    {
                        m_Slot.Value = guid;
                        m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                        if (m_PreviewData != null)
                        {
                            m_PreviewData.Content = GetItemSOFromGuid(m_Slot.Value);
                            m_PreviewData.NotifyPreviewChanged();
                        }
                    }
                }
            }
        }

        protected override void UpdatePreview()
        {
            if (m_PreviewItemSO != m_PreviewData.Content)
            {
                if (m_PreviewData.Content is ItemSO itemSO)
                {
                    m_PreviewItemSO = itemSO;
                }
                else if (m_PreviewData.Content == null)
                {
                    m_PreviewItemSO = null;
                }
            }
        }

        private ItemSO GetItemSOFromGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemSO itemSO = AssetDatabase.LoadAssetAtPath<ItemSO>(path);
            return itemSO;
        }
    }
}
*/