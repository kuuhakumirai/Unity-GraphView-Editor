using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Highlight != null)
        {
            Highlight.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Highlight != null)
        {
            Highlight.gameObject.SetActive(false);
        }
    }

    public TMP_Text Text;
    public Image Highlight;
    public Button Button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Highlight != null)
        {
            Highlight.gameObject.SetActive(false);
        }
    }
}
