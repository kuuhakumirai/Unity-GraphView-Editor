using System;

namespace GraphEditor
{
    public class PreviewData : IDisposable
    {
        public string PreviewName { get; set; }
        public UnityEngine.Object Content { get; set; }
        public Action OnPreviewChanged { get; set; }

        public void NotifyPreviewChanged()
        {
            OnPreviewChanged?.Invoke();
        }

        public void Dispose()
        {
            if (Content != null)
            {
                Content = null;
            }
        }
    }

}
