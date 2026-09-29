using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GraphEditor
{
    public static class GraphEditorUtils
    {
        private static readonly List<ScriptableObject> m_AssetsPrepareToSave = new();

        public static void AddToSaveList(ScriptableObject asset)
        {
            if (!m_AssetsPrepareToSave.Contains(asset))
            {
                m_AssetsPrepareToSave.Add(asset);
            }
        }

        public static void SaveAsset()
        {
            foreach (var asset in m_AssetsPrepareToSave)
            {
                MethodInfo info = asset.GetType().GetMethod("Serialize");
                info?.Invoke(asset, null);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
            }
            m_AssetsPrepareToSave.Clear();
        }

        private static readonly Dictionary<string, Type> m_NodeViews = new()
        {
            { "Property", typeof(PropertyNodeView) },
            { "Audio" , typeof(AudioNodeView) },
            { "Entry", typeof(EntryNodeView) },
            { "Image", typeof(ImageNodeView) },
            { "Int", typeof(IntNodeView)},
            { "Float", typeof(FloatNodeView) },
            { "Output", typeof(OutputNodeView) },
            { "Selection", typeof(SelectionNodeView) },
            { "Text", typeof(TextNodeView) },
            { "SelfSwitch", typeof(SelfSwitchNodeView) },
            { "IfVariable",  typeof(IfVariableNodeView) },
            { "IfSelfSwitch", typeof(IfSelfSwitchNodeView) },
            { "SetSelfSwitch", typeof(SetSelfSwitchNodeView) },
            // { "Item", typeof(ItemNodeView) },
            // { "Weapon", typeof(WeaponNodeView) },
            // { "Armor", typeof(ArmorNodeView) },
            { "ChangePackage", typeof(ChangePackageNodeView) },
            { "StringInput", typeof(StringInputNodeView) },
            { "FloatInput", typeof(FloatInputNodeView) },
        };

        public static ReadOnlyDictionary<string, Type> NodeViews { get; } = new(m_NodeViews);
    }

}
