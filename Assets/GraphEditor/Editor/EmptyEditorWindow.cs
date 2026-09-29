using UnityEditor;
using UnityEngine;

namespace GraphEditor
{
    public class EmptyEditorWindow : EditorWindow
    {
        private static GUIStyle titleStyle;

        private const int titleHeight = 120;
        private const int buttonWidth = 200;
        private const int buttonHeight = 22;

        private bool shouldCloseWindow;
        private Rect scrollArea;

        private void OnEnable()
        {
            scrollArea = new Rect(0, 0, 630, 300);
            shouldCloseWindow = false;
        }

        private void OnGUI()
        {
            OpenGraphFromPicker();

            titleStyle ??= new GUIStyle("Label") { alignment = TextAnchor.MiddleCenter, fontSize = 20 };

            Vector2 groupSize = new(scrollArea.width, 280);
            GUI.BeginGroup(new Rect((position.width / 2) - (groupSize.x / 2), (position.height / 2) - (groupSize.y / 2), groupSize.x, groupSize.y));

            GUI.Label(new Rect(0, 0, groupSize.x, titleHeight), "Welcome to Graph Editor!", titleStyle);

            float buttonX = groupSize.x / 2 - 5 - buttonWidth;

            if (GUI.Button(new Rect(buttonX, titleHeight, buttonWidth, buttonHeight), "Browse to open a Graph"))
            {
                OpenGraph();
            }

            buttonX += (buttonWidth + 10);


            if (GUI.Button(new Rect(buttonX, titleHeight, buttonWidth, buttonHeight), "Create new Graph"))
            {
                CreateGraph();
            }

            GUI.EndGroup();

            if (shouldCloseWindow)
            {
                Close();
            }

        }

        private void OpenGraph()
        {
            EditorGUIUtility.ShowObjectPicker<GraphObject>(null, false, string.Empty, EditorGUIUtility.GetControlID(FocusType.Passive));
        }

        private void OpenGraphFromPicker()
        {
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                UnityEngine.Object selectedObject = EditorGUIUtility.GetObjectPickerObject();
                if (selectedObject != null)
                {
                    OpenGraphAsset(selectedObject);
                }
            }
        }

        private void OpenGraphAsset(UnityEngine.Object selectedObject)
        {
            if (selectedObject is GraphObject go)
            {
                shouldCloseWindow = true;
                GraphWindow window = GetWindow<GraphWindow>();
                window.OpenAsset(go);
            }
        }

        private void CreateGraph()
        {
            GraphWindow window = GetWindow<GraphWindow>();
            window.titleContent = new("Graph Editor");
            if (CreateGraphAsset())
            {
                shouldCloseWindow = true;
            }
        }

        private bool CreateGraphAsset()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Graph", "New Editor Graph", "asset", null);
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            GraphObject go = CreateInstance<GraphObject>();
            go.Graph = new();
            AssetDatabase.CreateAsset(go, path);
            OpenGraphFromPath(path);
            return true;
        }

        private void OpenGraphFromPath(string path)
        {
            path = path.Replace(Application.dataPath, "Assets");
            UnityEngine.Object graphObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            OpenGraphAsset(graphObject);
        }


        [MenuItem("Graph Window/Graph Window")]
        public static void OpenGraphWindow()
        {
            EmptyEditorWindow window = GetWindow<EmptyEditorWindow>();
            window.titleContent = new("Graph Editor");
        }
    }

}
