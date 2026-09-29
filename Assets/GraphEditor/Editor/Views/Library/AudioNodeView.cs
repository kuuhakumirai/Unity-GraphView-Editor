using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class AudioNodeView : BasicNodeView
    {
        private ObjectField objectField;
        protected override bool HasPreview => true;
        private AudioClip m_PreviewAudioClip;

        private SlotData m_Slot;

        public AudioNodeView() : base()
        {
        }

        public override void Init(NodeData data)
        {
            base.Init(data);

            objectField = new ObjectField()
            {
                objectType = typeof(AudioClip),
            };
            contentContainer.Add(objectField);
            m_PreviewImage.AddToClassList("Hidden");
            m_PreviewFiller.AddToClassList("Hidden");

            VisualElement previewAudio = new()
            {
                name = "preview-Audio",
            };

            {
                VisualElement leftFiller = new()
                {
                    name = "left"
                };

                VisualElement playButton = new()
                {
                    name = "icon-Play",
                };
                playButton.AddManipulator(new Clickable(() =>
                {
                    PlayCurrentAudio();
                }));
                leftFiller.Add(playButton);
                previewAudio.Add(leftFiller);
            }
            {
                VisualElement rightFiller = new()
                {
                    name = "right"
                };

                VisualElement pauseButton = new()
                {
                    name = "icon-Stop",
                };
                pauseButton.AddManipulator(new Clickable(() =>
                {
                    StopCurrentAudio();
                }));
                rightFiller.Add(pauseButton);
                previewAudio.Add(rightFiller);
            }
            var contents = this.Q("contents");
            contents.Add(previewAudio);

            m_Slot = data.Slots.Where(x => x.Name == "V").FirstOrDefault();

            if (!string.IsNullOrEmpty(m_Slot.Value))
            {
                m_PreviewData.Content = GetAudioClipFromGuid(m_Slot.Value);
                if (m_PreviewData.Content is AudioClip audioClip)
                {
                    m_PreviewAudioClip = audioClip;
                    objectField.SetValueWithoutNotify(m_PreviewAudioClip);
                    m_PreviewData.NotifyPreviewChanged();
                }
            }

            objectField.RegisterValueChangedCallback(OnValueChanged);
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
                            m_PreviewData.Content = GetAudioClipFromGuid(m_Slot.Value);
                            m_PreviewData.NotifyPreviewChanged();
                        }
                    }
                }
            }
        }

        private void PlayCurrentAudio()
        {
            if (m_PreviewAudioClip != null)
            {
                NodeUtils.PlayAudioClip(m_PreviewAudioClip);
            }
        }

        private void StopCurrentAudio()
        {
            if (m_PreviewAudioClip != null)
            {
                if (NodeUtils.CurrentAudioClip == m_PreviewAudioClip)
                {
                    NodeUtils.StopAllAudioClip();
                }
            }
        }

        private AudioClip GetAudioClipFromGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            return audioClip;
        }

        protected override void UpdatePreview()
        {
            if (m_PreviewAudioClip != m_PreviewData.Content)
            {
                if (m_PreviewData.Content is AudioClip audioClip)
                {
                    m_PreviewAudioClip = audioClip;
                }
                else if (m_PreviewData.Content == null)
                {
                    m_PreviewAudioClip = null;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            m_PreviewAudioClip = null;
        }
    }
}