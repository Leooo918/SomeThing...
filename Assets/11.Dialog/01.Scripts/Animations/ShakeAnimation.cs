using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeAnimation : MonoBehaviour
{
    private Sequence seq = null;
    private Vector2 originPos;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        originPos = rect.anchoredPosition;

        StartCoroutine("ShakeRoutine");
    }

    private void OnDisable()
    {
        StopCoroutine("ShakeRoutine");
    }

    IEnumerator ShakeRoutine()
    {
        while (true)
        {
            if (gameObject.activeSelf == false) yield break;
            rect.DOAnchorPosX(originPos.x + 10f, 0.1f);

            yield return new WaitForSeconds(0.1f);

            if (gameObject.activeSelf == false) yield break;
            rect.DOAnchorPosX(originPos.x - 10f, 0.1f);

            yield return new WaitForSeconds(0.1f);
        }

    }

}
