using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GraphEditor
{
    public class SBlackboard : Blackboard, IDisposable
    {
        private const string PATH = "BlackboardObject.asset";
        private BlackboardObject m_BlackboardObject;
        private readonly BlackboardData m_BlackboardData;
        private GraphEditorView m_GraphEditorView;

        public GraphEditorView GraphEditorView
        {
            get => m_GraphEditorView;
            init
            {
                m_GraphEditorView = value;
                if (m_GraphEditorView != null)
                {
                    m_GraphEditorView.SaveRequested += OnSave;
                }
            }
        }
        public BlackboardData BlackboardData => m_BlackboardData;
        public Action OnBlackboardChanged { get; private set; }
        public string BlackboardTitle { get; init; }
        internal GenericMenu AddBlackboardItemMenu => m_AddBlackboardItemMenu;
        private GenericMenu m_AddBlackboardItemMenu;

        private string m_LastSerializedFileContents;

        void InitializeAddBlackboardItemMenu()
        {
            m_AddBlackboardItemMenu = new GenericMenu();
            foreach (var item in BlackboardUtils.NameToAddType)
            {
                string propertyName = item.Key;
                string displayedName = BlackboardUtils.TypeToDisplayed[item.Value];
                m_AddBlackboardItemMenu.AddItem(new GUIContent(displayedName), false, () => AddItem(item.Value));
            }
        }

        public SBlackboard() : base()
        {
            m_BlackboardObject = AssetDatabase.LoadAssetAtPath<BlackboardObject>(BlackboardUtils.PackageRelativePath + "/" + PATH);
            if (m_BlackboardObject == null)
            {
                m_BlackboardObject = ScriptableObject.CreateInstance<BlackboardObject>();
                m_BlackboardObject.Blackboard = new();
                AssetDatabase.CreateAsset(m_BlackboardObject, BlackboardUtils.PackageRelativePath + "/" + PATH);
            }
            m_BlackboardObject.Deserialize();
            m_BlackboardData = m_BlackboardObject.Blackboard;
            InitializeAddBlackboardItemMenu();
            addItemRequested += OnAddButtonClicked;
            OnBlackboardChanged += AddToSaveList;
            OnBlackboardChanged += UpdateTitle;
            m_LastSerializedFileContents = GraphUtils.Serialize(m_BlackboardData);
            Init();
        }

        private void Init()
        {
            if (m_BlackboardData.Fields.Count > 0)
            {
                foreach (var item in m_BlackboardData.Fields)
                {
                    if (item is FieldData data)
                    {
                        SBlackboardField blackboardField = new(data)
                        {
                            text = data.Name,
                            typeText = BlackboardUtils.TypeToDisplayed[data.GetType()],
                            viewDataKey = data.Guid
                        };
                        contentContainer.Add(blackboardField);
                    }
                }
            }
        }

        private void OnAddButtonClicked(Blackboard _)
        {
            m_AddBlackboardItemMenu.ShowAsContext();
        }

        private void AddItem(Type type)
        {
            FieldData fieldData = (FieldData)Activator.CreateInstance(type);
            SBlackboardField blackboardField = new(fieldData)
            {
                text = fieldData.Name,
                typeText = BlackboardUtils.TypeToDisplayed[fieldData.GetType()],
                viewDataKey = fieldData.Guid
            };
            contentContainer.Add(blackboardField);
            m_BlackboardData.AddField(fieldData);
            OnBlackboardChanged?.Invoke();
        }

        public void RemoveItem(SBlackboardField field)
        {
            FieldData fieldData = m_BlackboardData.FindDataFromId(field.viewDataKey);
            if (fieldData != null)
            {
                IEnumerable<Node> nodesToRemove = graphView.nodes.Where(a => fieldData.Alter.Contains(a.viewDataKey));
                if (nodesToRemove.Count() > 0)
                {
                    foreach (var item in nodesToRemove)
                    {
                        Port port = item.outputContainer.Q<Port>();
                        IEnumerable<Edge> edgesToRemove = graphView.edges.Where(a => a.output.viewDataKey == port.viewDataKey);
                        graphView.DeleteElements(edgesToRemove);
                    }
                    graphView.DeleteElements(nodesToRemove);
                }
                m_BlackboardData.RemoveField(fieldData);
                OnBlackboardChanged?.Invoke();
            }
        }

        public void RenameItem(SBlackboardField field)
        {
            FieldData fieldData = m_BlackboardData.FindDataFromId(field.viewDataKey);
            if (fieldData != null)
            {
                string name = field.text;
                m_BlackboardData.RenameField(fieldData, name);
                IEnumerable<Node> nodesToRename = graphView.nodes.Where(a => fieldData.Alter.Contains(a.viewDataKey));
                if (nodesToRename.Count() > 0)
                {
                    foreach (Node item in nodesToRename)
                    {
                        if (item is TokenNode node)
                        {
                            node.output.portName = name;
                        }
                    }
                }
                OnBlackboardChanged?.Invoke();
            }
        }

        public VisualElement FindItem(string guid)
        {
            return contentContainer.Children().FirstOrDefault(x => x.viewDataKey == guid);
        }

        public void RemoveAlter(string alter)
        {
            FieldData fieldData = m_BlackboardData.FindDataFromAlter(alter);
            VisualElement visualElement = FindItem(fieldData.Guid);
            if (visualElement is SBlackboardField field)
            {
                field.FieldData.RemoveAlter(alter);
                OnBlackboardChanged?.Invoke();
            }
        }

        private void AddToSaveList()
        {
            GraphEditorUtils.AddToSaveList(m_BlackboardObject);
        }

        private void UpdateTitle()
        {
            string title = BlackboardTitle;
            if (GraphHasChangedSinceLastSerialization())
            {
                title += "*";
            }
            this.title = title;
        }

        private bool GraphHasChangedSinceLastSerialization()
        {
            Assert.IsTrue(m_BlackboardObject != null && m_BlackboardData != null); // this should be checked by calling code
            string currentGraphJson = GraphUtils.Serialize(m_BlackboardData);
            return !string.Equals(currentGraphJson, m_LastSerializedFileContents, StringComparison.Ordinal);
        }

        private void OnSave()
        {
            m_LastSerializedFileContents = GraphUtils.Serialize(m_BlackboardData);
            UpdateTitle();
        }

        public void Dispose()
        {
            m_BlackboardObject = null;
            addItemRequested -= OnAddButtonClicked;
            OnBlackboardChanged -= AddToSaveList;
            OnBlackboardChanged -= UpdateTitle;
            GraphEditorView.SaveRequested -= OnSave;
        }
    }
}