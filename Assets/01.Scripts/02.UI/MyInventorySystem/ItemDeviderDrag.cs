using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDeviderDrag : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Vector2 clickPoint;
    private Vector2 diffrence;
    private Camera mainCamera = null;
    private RectTransform rectTransform = null;
    private RectTransform itemDevider = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        itemDevider = GameObject.Find("ItemDeviderHelper").GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            rectTransform.anchoredPosition = eventData.position + new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize) - clickPoint;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickPoint = eventData.position - rectTransform.anchoredPosition;
    }
}
