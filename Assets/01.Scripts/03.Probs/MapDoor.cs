using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapDoor : MonoBehaviour
{
    private GameObject open = null;
    private GameObject closed = null;

    private bool isOpened = false;

    private void Awake()
    {
        open = transform.Find("Open").gameObject;
        closed = transform.Find("Closed").gameObject;

        open.SetActive(false);
    }


    public void OpenDoor()
    {
        isOpened = true;

        open.SetActive(true);                                                                                                                                                                                
        closed.SetActive(false);
    }

    public void CloseDoor()
    {
        isOpened = false;

        open.SetActive(false);
        closed.SetActive(true);

    }
}