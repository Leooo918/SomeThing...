using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PushAnimation : MonoBehaviour
{
    private Sequence seq = null;
    private RectTransform suba = null;
    private RectTransform amelia = null;
    private RectTransform portal = null;

    private void Awake()
    {
        suba = transform.Find("Suba").GetComponent<RectTransform>();
        amelia = transform.Find("Amelia").GetComponent<RectTransform>();
        portal = transform.Find("Portal").GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        seq = DOTween.Sequence();

        seq.Append(suba.DOAnchorPosX(190, 0.3f))
            .Insert(0.2f, amelia.DOAnchorPosX(619, 0.2f))
            .Insert(0.2f, amelia.DORotate(new Vector3(0, 0, -90), 0.2f))
            .Insert(0.5f, amelia.DOScale(0, 0.5f))
            .Insert(0.4f, suba.DOJumpAnchorPos(new Vector2(-585, -68), 5, 3, 1f));
    }
}
