using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeizureAnimation : CharacterAnimation
{
    private Sequence seq;
    private Vector2 originPos;

    public override void Animation()
    {
        seq = DOTween.Sequence();

        originPos = rect.anchoredPosition;

        for (int i = 0; i < 10; i++)
        {
            seq.Append(rect.DOAnchorPosY(originPos.y + 100, 0.1f))
                .Append(rect.DOAnchorPosY(originPos.y - 100, 0.1f));
        }
    }
}
