using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubaAmeliaFromPortal : MonoBehaviour
{

    private Sequence seq = null;
    private RectTransform suba = null;
    private RectTransform amelia = null;


    private void Awake()
    {
        amelia = transform.Find("Amelia").GetComponent<RectTransform>();
        suba = transform.Find("Suba").GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        seq = DOTween.Sequence();

        seq.Append(amelia.DOScale(1, 0.3f))
            .Join(suba.DOScale(1, 0.3f))
            .Join(suba.DOAnchorPosX(-614 ,0.3f))
            .Join(amelia.DOAnchorPosX(64 ,0.3f));
    }
}
