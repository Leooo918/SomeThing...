using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipUI : MonoBehaviour
{

    private RectTransform toolTipBackground = null;
    private TextMeshProUGUI textMeshPro = null;
    private RectTransform rectTransform = null;

    [SerializeField]private string toolTipText = "";
    [SerializeField]private RectTransform canvasRectTransform = null;

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        toolTipBackground = transform.Find("Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        SetText("FuckYou");
    }

    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 PeddingSize = new Vector2(8, 8);

        toolTipBackground.sizeDelta = textSize + PeddingSize;
    }

    private void Update()
    {
        Vector2 anchoredPos =  Input.mousePosition;

        anchoredPos = new Vector2(Mathf.Clamp(anchoredPos.x, 0, canvasRectTransform.rect.width - toolTipBackground.rect.width), Mathf.Clamp(anchoredPos.y, 0, canvasRectTransform.rect.height - toolTipBackground.rect.height));

        rectTransform.anchoredPosition = anchoredPos;

        SetText(toolTipText);
    }

    public void ShowToolTip(string toolTipText)
    {
        gameObject.SetActive(true);
        SetText(toolTipText);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
