using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckProfileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isMouseOnObject = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isMouseOnObject == false)
        {
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOnObject = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOnObject = false;
    }
}
