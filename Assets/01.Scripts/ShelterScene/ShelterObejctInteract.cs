using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterObejctInteract : MonoBehaviour
{
    private ShelterObject shelterObj = null;
    public bool isInteracting = false;

    private void Awake()
    {
        shelterObj = GetComponentInParent<ShelterObject>();
    }

    private void Update()
    {
        if (isInteracting == true && Input.GetKeyDown(KeyCode.F))
        {
            shelterObj.UseObject();
            isInteracting = false;
            shelterObj.interactUI.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ShelterPlayer>(out var a))
        {
            isInteracting = false;
            shelterObj.interactUI.SetActive(false);
        }
    }
}
