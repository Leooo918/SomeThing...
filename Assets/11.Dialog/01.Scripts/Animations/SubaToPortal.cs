using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubaToPortal : MonoBehaviour
{
    private Sequence seq = null;
    private RectTransform suba = null;


    private void Awake()
    {
        suba = transform.Find("Suba").GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        seq = DOTween.Sequence();

        seq.Append(suba.DOAnchorPosX(650, 0.5f))
            .Insert(0, suba.DORotate(new Vector3(0, 0, -90), 0.5f))
            .Append(suba.DOScale(0, 0.3f));
    }
}
