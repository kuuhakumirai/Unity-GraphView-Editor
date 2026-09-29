using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

namespace GraphEditor.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueBox : UIBehaviour
    {
        public TMP_Text Name;
        public TMP_Text Content;
        public Image Head;
        [SerializeField]
        private SelectionUI[] m_Selections;
        public SelectionUI[] Selections => m_Selections;

        public Action OnUIClicked { get; set; }

        protected override void Awake()
        {
            GetComponent<CanvasGroup>().alpha = 0.0f;
            foreach (var item in m_Selections)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void UIClicked()
        {
            OnUIClicked?.Invoke();
        }
    }
}
