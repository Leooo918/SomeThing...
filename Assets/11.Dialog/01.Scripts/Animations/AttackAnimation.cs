using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AttackAnimation : MonoBehaviour
{
    private Sequence seq = null;
    private RectTransform suba = null;
    private RectTransform amelia = null;
    private RectTransform katana = null;

    private void Awake()
    {
        suba = transform.Find("Suba").GetComponent<RectTransform>();
        amelia = transform.Find("Amelia").GetComponent<RectTransform>();
        katana = suba.transform.Find("Katana").GetComponent<RectTransform>();

        print(suba);
        print(amelia);
        print(katana);
    }

    public void OnEnable()
    {
        seq = DOTween.Sequence();

        seq.Append(suba.DOJumpAnchorPos(new Vector2(-250f, -85), 300, 1, 0.5f))
            .Append(katana.DORotate(new Vector3(0, 0, 100), 0.2f))  
            .Append(amelia.DORotate(new Vector3(0, 0, -90), 0.5f))
            .Join(amelia.DOAnchorPosY(-638f, 0.5f));
    }
}
