using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapLever : MonoBehaviour
{
    private GameObject leftSideLever = null;
    private GameObject rightSideLever = null;

    public UnityEvent onLeftSide = null;
    public UnityEvent onRightSide = null;

    [SerializeField] private bool isLeftSide = true;

    private void Awake()
    {
        leftSideLever = transform.Find("LeftSide").gameObject;
        rightSideLever = transform.Find("RightSide").gameObject;
    }

    private void Update()
    {
        if(isLeftSide == true)
        {
            leftSideLever.SetActive(true);
            rightSideLever.SetActive(false);
        }
        else
        {
            leftSideLever.SetActive(false);
            rightSideLever.SetActive(true);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.position.x - transform.position.x > 0)
        {
            isLeftSide = true;
            onLeftSide?.Invoke();
        }
        else
        {
            isLeftSide = false;
            onRightSide?.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
