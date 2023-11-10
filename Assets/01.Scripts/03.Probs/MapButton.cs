using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapButton : MonoBehaviour
{
    private Transform pressedBtn = null;
    private Transform releasedBtn = null;

    [SerializeField] private float btnReleaseTime = 1f;
    private float btnReleaseTimeDown = 0f;

    private bool isPressing = false;

    public UnityEvent onPressedEvent = null;
    public UnityEvent onReleaseEvent = null;

    private void Awake()
    {
        pressedBtn = transform.Find("PressedBtn");
        releasedBtn = transform.Find("ReleaseBtn");

        pressedBtn.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(btnReleaseTimeDown > 0 && isPressing == false)
        {
            btnReleaseTimeDown -= Time.deltaTime;
        }

        if(isPressing == true && pressedBtn.gameObject.activeSelf == false)
        {
            pressedBtn.gameObject.SetActive(true);
            releasedBtn.gameObject.SetActive(false);
            onPressedEvent?.Invoke();
        }

        if(btnReleaseTimeDown <= 0 && isPressing == false && releasedBtn.gameObject.activeSelf == false)
        {
            pressedBtn.gameObject.SetActive(false);
            releasedBtn.gameObject.SetActive(true);
            onReleaseEvent?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isPressing = true;
        btnReleaseTimeDown = btnReleaseTime;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPressing = false;
        btnReleaseTimeDown = btnReleaseTime;
    }
}
