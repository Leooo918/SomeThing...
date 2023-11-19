using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShelterSceneManager : MonoBehaviour
{
    public static ShelterSceneManager instance = null;

    private Camera mainCam = null;
    private ShelterObject[] shelterObjects;
    public ShelterPlayer player = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
        }

        Vector2 pointerPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colls = Physics2D.OverlapCircleAll(pointerPos, 0.1f);

        foreach (Collider2D coll in colls)
        {
            Debug.Log("¾ßÈ£");
            if (coll.TryGetComponent<ShelterObject>(out ShelterObject obj))
            {
                obj.OnPointerEnter();
            }
        }
    }
}
