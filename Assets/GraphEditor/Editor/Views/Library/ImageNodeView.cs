using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class ImageNodeView : BasicNodeView
    {
        private ObjectField objectField;
        protected override bool HasPreview => true;
        private SlotData m_Slot;

        public ImageNodeView() : base()
        {
        }

        public override void Init(NodeData data)
        {
            base.Init(data);

            objectField = new ObjectField()
            {
                objectType = typeof(Texture),
            };
            contentContainer.Add(objectField);

            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();

            objectField.RegisterValueChangedCallback(OnValueChanged);

            if (!string.IsNullOrEmpty(m_Slot.Value))
            {
                m_PreviewData.Content = GetTextureFromGuid(m_Slot.Value);
                if (m_PreviewData.Content is Texture texture)
                {
                    m_PreviewImage.image = texture;
                    objectField.SetValueWithoutNotify(texture);
                    m_PreviewData.NotifyPreviewChanged();
                }
            }
        }

        private void OnValueChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue == null)
            {
                if (m_Slot.Value != null)
                {
                    m_Slot.Value = "";
                    m_Slot.Owner.Owner.Owner.RegisterCompleteObjectUndo("Change " + node.Name);
                    if (m_PreviewData != null)
                    {
                        m_PreviewData.Content = new Texture2D(200, 200);
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
                            m_PreviewData.Content = GetTextureFromGuid(m_Slot.Value);
                            m_PreviewData.NotifyPreviewChanged();
                        }
                    }
                }
            }
        }

        protected override void UpdatePreview()
        {
            if (m_PreviewImage.image != m_PreviewData.Content)
            {
                if (m_PreviewData.Content is Texture texture)
                {
                    m_PreviewImage.image = texture;
                }
            }
        }

        protected Texture GetTextureFromGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            if (texture != null)
            {
                return texture;
            }
            return new Texture2D(200, 200);
        }

        public override void Dispose()
        {
            base.Dispose();
            m_PreviewImage = null;
        }
    }
}
