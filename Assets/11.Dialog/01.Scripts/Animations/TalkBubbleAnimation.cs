using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkBubbleAnimation : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("AnimationRoutine");
    }

    private void OnDisable()
    {
        StopCoroutine("AnimationRoutine");
    }

    IEnumerator AnimationRoutine()
    {
        RectTransform imageRect = transform.Find("Image").GetComponent<RectTransform>();
        while (true)
        {
            if(imageRect.gameObject.activeSelf == false) yield break;
            imageRect.eulerAngles = new Vector3(0, 0, -50);
            
            yield return new WaitForSeconds(0.2f);
            
            if (imageRect.gameObject.activeSelf == false) yield break;
            imageRect.eulerAngles = new Vector3(0, 0,  0);

            yield return new WaitForSeconds(0.2f); 
        }
    }
}
