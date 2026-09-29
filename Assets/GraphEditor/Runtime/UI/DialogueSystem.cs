using GraphEditor.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphEditor.UI
{
    [RequireComponent(typeof(GraphInstance))]
    public class DialogueSystem : MonoBehaviour
    {
        public DialogueBox DialogueBox;
        private GraphInstance instance;

        [SerializeField]
        private bool isAutoPlay = false;

        void Start()
        {
            instance = GetComponent<GraphInstance>();
            if (!instance.Compiler.IsGraphValidate)
            {
                Debug.LogError("The Graph has Error!");
                return;
            }
            if (DialogueBox == null)
            {
                DialogueBox = GameObject.Find("Canvas/DialogueBox").GetComponent<DialogueBox>();
                if (DialogueBox == null)
                {
                    Debug.LogError("There is no Dialogue Box in Scene!");
                    return;
                }
            }
            if (isAutoPlay)
            {
                PlayGraph();
            }
        }


        private IEnumerator WaitForUIClick()
        {
            bool isClicked = false;

            void ClickHandler() => isClicked = true;
            DialogueBox.OnUIClicked += ClickHandler;

            while (!isClicked)
            {
                yield return null;
            }

            DialogueBox.OnUIClicked -= ClickHandler;
        }


        public void PlayGraph()
        {
            StartCoroutine(PlayGraphCoroutine());
        }

        private IEnumerator PlayGraphCoroutine()
        {
            Queue<NodeData> nodeQueue = new();

            foreach (var node in instance.Compiler.SortedNodes.Keys)
            {
                nodeQueue.Enqueue(node);
            }

            while (nodeQueue.Count > 0)
            {
                NodeData currentNode = nodeQueue.Dequeue();

                yield return ConvertSubTypeToPlay(currentNode);

                if (currentNode is SelectionNode selectionNode)
                {
                    CoroutineWithData coroutine = new(this, ShowSelectionsAndWaitForClick(selectionNode));
                    yield return coroutine.coroutine;

                    int selectedBranchIndex = coroutine.GetResult<int>();

                    var selectedOutSlot = selectionNode.Slots
                        .Where(slot => slot.Name.Contains("Out"))
                        .ElementAtOrDefault(selectedBranchIndex);

                    if (selectedOutSlot != null && selectedOutSlot.Connections != null)
                    {
                        foreach (var connectionGuid in selectedOutSlot.Connections)
                        {
                            var nextNode = FindNodeByInputGuid(connectionGuid);
                            if (nextNode != null)
                            {
                                nodeQueue.Enqueue(nextNode);
                            }
                        }
                        continue;
                    }
                }

                if (currentNode.Slots != null)
                {
                    nodeQueue.Clear();
                    foreach (var outSlot in currentNode.Slots.Where(s => s.Name.Contains("Out")))
                    {
                        if (outSlot.Connections != null)
                        {
                            foreach (var connectionGuid in outSlot.Connections)
                            {
                                var nextNode = FindNodeByInputGuid(connectionGuid);
                                if (nextNode != null)
                                {
                                    nodeQueue.Enqueue(nextNode);
                                }
                            }
                        }
                    }
                }

            }
        }

        private NodeData FindNodeByInputGuid(string inputGuid)
        {
            return instance.Compiler.SortedNodes
                .Where(kvp => kvp.Value.Any(slot =>
                    slot.Name == "In" && slot.Guid == inputGuid))
                .Select(kvp => kvp.Key)
                .FirstOrDefault();
        }

        private IEnumerator ConvertSubTypeToPlay(NodeData node)
        {
            switch (node)
            {
                case EntryNode:
                    break;
                case OutputNode:
                    HideDialogueBox();
                    break;
                case SpeakNode speak:
                    {
                        yield return PlaySpeakNodeAndWaitForClick(speak);
                    }
                    break;
                default:
                    break;
            }
        }

        private IEnumerator ShowSelectionsAndWaitForClick(SelectionNode selection)
        {
            var selections = selection.GetAvailableSelections();
            var strings = selection.GetSelectionText(selections, instance);

            for (int i = 0; i < selections.Count; i++)
            {
                DialogueBox.Selections[i].Text.text = strings[i];
                DialogueBox.Selections[i].gameObject.SetActive(true);
            }

            CoroutineWithData cd = new(this, WaitForSelectionClick());
            yield return cd.coroutine;

            int selectedIndex = cd.GetResult<int>();

            for (int i = selections.Count - 1; i >= 0; i--)
            {
                DialogueBox.Selections[i].gameObject.SetActive(false);
            }

            yield return selectedIndex;
        }

        private IEnumerator WaitForSelectionClick()
        {
            int clickedIndex = -1;
            bool isClicked = false;

            for (int i = 0; i < DialogueBox.Selections.Length; i++)
            {
                int index = i; 
                if (DialogueBox.Selections[i].Button != null && DialogueBox.Selections[i].gameObject.activeSelf)
                {
                    DialogueBox.Selections[i].Button.onClick.RemoveAllListeners();
                    DialogueBox.Selections[i].Button.onClick.AddListener(() =>
                    {
                        clickedIndex = index;
                        isClicked = true;
                    });
                }
            }

            while (!isClicked)
            {
                yield return null;
            }

            foreach (SelectionUI selection in DialogueBox.Selections)
            {
                if (selection.Button != null)
                {
                    selection.Button.onClick.RemoveAllListeners();
                }
            }

            yield return clickedIndex;
        }

        private void ShowDialogueBox()
        {
            if (DialogueBox.GetComponent<CanvasGroup>().alpha != 1.0f)
            {
                DialogueBox.GetComponent<CanvasGroup>().alpha = 1.0f;
            }
        }

        private void HideDialogueBox()
        {
            DialogueBox.GetComponent<CanvasGroup>().alpha = 0.0f;
            DialogueBox.Name.text = string.Empty;
            DialogueBox.Head.sprite = null;
        }

        private IEnumerator PlaySpeakNodeAndWaitForClick(SpeakNode node)
        {
            ShowDialogueBox();
            string guid = node.GetSpeakerHead(instance);
            UnityEngine.Object obj = instance.Compiler.AddressablePreLoader.SpriteAssets[guid];
            if (obj is Sprite sprite)
            {
                DialogueBox.Head.sprite = sprite;
            }
            DialogueBox.Content.text = node.GetSpeakText(instance);
            DialogueBox.Name.text = node.GetSpeaker(instance);
            yield return WaitForUIClick();
        }

        private void OnDisable()
        {
        }
    }


    public class CoroutineWithData
    {
        public Coroutine coroutine;
        public object result;
        private readonly IEnumerator target;

        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            this.coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (target.MoveNext())
            {
                result = target.Current;
                yield return result;
            }
        }

        public T GetResult<T>()
        {
            if (result is T t)
                return t;
            else
                return default;
        }
    }
}

