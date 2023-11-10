using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapLava : MonoBehaviour
{
    public UnityEvent onInteract = null;
    private bool isInteract = false;
    private bool isInteracted = false;
    private GameObject interactImg = null;

    private void Awake()
    {
        interactImg = transform.Find("PressFImage").gameObject;
        interactImg.SetActive(false);
    }

    private void Update()
    {
        if(isInteract && Input.GetKeyDown(KeyCode.F))
        {
            isInteracted = true;
            isInteract = false;
            interactImg.SetActive(false);

            onInteract?.Invoke();
            Debug.Log("µÈ");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInteracted == true) return;
        interactImg.SetActive(true);
        isInteract = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isInteracted == true) return;
        interactImg.SetActive(false);
        isInteract = false;
    }
}
