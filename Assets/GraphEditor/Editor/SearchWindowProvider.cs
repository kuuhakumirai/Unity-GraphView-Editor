using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphEditor
{
    [Serializable]
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public struct NodeEntry
        {
            public string[] title;
            public Type type;
            public int compatibleSlotId;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var nodeEntries = new List<NodeEntry>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(NodeData))))
                    {
                        if (type.GetCustomAttributes(typeof(TitleAttribute), false) is TitleAttribute[] attributes && attributes.Length > 0)
                        {
                            AddEntries(type, attributes[0].title, nodeEntries);
                        }
                    }
                }
            }
            nodeEntries.Sort((entry1, entry2) =>
            {
                for (var i = 0; i < entry1.title.Length; i++)
                {
                    if (i >= entry2.title.Length)
                        return 1;
                    var value = entry1.title[i].CompareTo(entry2.title[i]);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (entry1.title.Length != entry2.title.Length && (i == entry1.title.Length - 1 || i == entry2.title.Length - 1))
                            return entry1.title.Length < entry2.title.Length ? -1 : 1;
                        return value;
                    }
                }
                return 0;
            });

            var groups = new List<string>();

            var tree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
        };

            foreach (var nodeEntry in nodeEntries)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < nodeEntry.title.Length - 1; i++)
                {
                    var group = nodeEntry.title[i];
                    if (i >= groups.Count)
                    {
                        // The current group path matches a prefix of the current entry's group path, so we add the
                        // rest of the group path from the current entry.
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        // A prefix of the current group path matches a prefix of the current entry's group path,
                        // so we remove everyfrom from the point where it doesn't match anymore, and then add the rest
                        // of the group path from the current entry.
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                // Create new group entries as needed.
                // If we don't need to modify the group path, `createIndex` will be `int.MaxValue` and thus the loop won't run.
                for (var i = createIndex; i < nodeEntry.title.Length - 1; i++)
                {
                    var group = nodeEntry.title[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(nodeEntry.title.Last())) { level = nodeEntry.title.Length, userData = nodeEntry.type });
            }

            return tree;
        }

        private void AddEntries(Type type, string[] title, List<NodeEntry> nodeEntries)
        {
            nodeEntries.Add(new NodeEntry
            {
                type = type,
                title = title,
                compatibleSlotId = -1
            });
        }


        public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

        public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler; //delegate�ص�����

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null)
            {
                return false;
            }
            return OnSelectEntryHandler(searchTreeEntry, context);
        }
    }

    public class SearcherProvider : SearchWindowProvider
    {

    }
}
