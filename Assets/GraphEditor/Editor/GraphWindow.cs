using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GraphEditor
{
    public class GraphWindow : EditorWindow
    {
        private GraphEditorView m_GraphEditorView;

        public GraphEditorView GraphEditorView
        {
            get => m_GraphEditorView;
            set
            {
                m_GraphEditorView = value;
                if (m_GraphEditorView != null)
                {
                    GraphEditorView.SaveRequested += () => SaveAsset();
                    rootVisualElement.Add(m_GraphEditorView);
                }
            }
        }

        [SerializeField]
        string m_LastSerializedFileContents;

        [SerializeField]
        string m_Selected;

        public string SelectedGuid
        {
            get { return m_Selected; }
            private set
            {
                m_Selected = value;
            }
        }

        public Vector2 ScreenPosition => position.position;

        [SerializeField]
        private GraphObject m_GraphObject;

        internal GraphObject GraphObject
        {
            get => m_GraphObject;
            set
            {
                m_GraphObject = value;
            }
        }

        private void Update()
        {
            bool updateTitle = false;
            if (GraphObject == null && SelectedGuid != null)
            {
                var guid = SelectedGuid;
                SelectedGuid = null;
                Initialize(guid);
            }
            if (GraphObject == null)
            {
                Close();
                return;
            }
            GraphData graph = GraphObject.Graph;
            if (graph == null)
            {
                return;
            }
            GraphEditorView ??= new(this, graph);
            if (GraphObject.IsDirty)
            {
                updateTitle = true;
                GraphObject.IsDirty = false;
            }
            if (updateTitle)
            {
                UpdateTitle();
            }
            
            // GraphEditorView.HandleGraphChanges();
            // GraphObject.Graph.ClearChanges();
        }

        private void SaveAsset()
        {
            if (GraphObject != null)
            {
                GraphEditorUtils.AddToSaveList(GraphObject);
            }
            GraphEditorUtils.SaveAsset();
            if (GraphObject.Graph != null)
            {
                m_LastSerializedFileContents = GraphUtils.Serialize(GraphObject.Graph);
            }
            UpdateTitle();
        }

        public void OpenAsset(GraphObject go)
        {
            SelectedGuid = GetGuid(go);
            this.titleContent = new(go.name);
        }

        private static string GetGuid(Object obj)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string guid, out _);

            return guid;
        }

        private void Initialize(string assetGuid)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GraphObject>(AssetDatabase.GUIDToAssetPath(assetGuid));
            if (asset == null)
            {
                return;
            }
            if (SelectedGuid == assetGuid)
            {
                return;
            }
            SelectedGuid = assetGuid;
            var path = AssetDatabase.GetAssetPath(asset);
            var extension = Path.GetExtension(path);
            if (extension == null)
            {
                return;
            }
            GraphObject = asset;
            GraphObject.Graph = new();
            GraphObject.Deserialize();
        }

        private void UpdateTitle()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(SelectedGuid);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);

            string title = assetName;
            if (GraphObject == null || GraphObject.Graph == null)
            {
                title += " (nothing loaded)";
            }
            else
            {
                if (GraphHasChangedSinceLastSerialization())
                {
                    title += "*";
                }
                if (!AssetFileExists())
                {
                    title += " (deleted)";
                }
            }
            titleContent = new GUIContent(title);
        }

        private bool AssetFileExists()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(SelectedGuid);
            return File.Exists(assetPath);
        }

        private bool GraphHasChangedSinceLastSerialization()
        {
            Assert.IsTrue(GraphObject!= null && GraphObject.Graph != null); // this should be checked by calling code
            string currentGraphJson = GraphUtils.Serialize(GraphObject.Graph);
            return !string.Equals(currentGraphJson, m_LastSerializedFileContents, System.StringComparison.Ordinal);
        }

        private void OnDisable()
        {
            m_GraphEditorView?.Dispose();
            m_GraphEditorView = null;
            m_GraphObject = null;
            NodeUtils.StopAllAudioClip();
            Resources.UnloadUnusedAssets();
        }
    }
}

