using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RunAnimation : CharacterAnimation
{
    private Sequence seq = null;
    private Vector2 originPos;

    public override void Animation()
    {
        seq = DOTween.Sequence();

        originPos = rect.anchoredPosition;

        seq.Append(rect.DOAnchorPosX(originPos.x + 2f, 0.5f))
            .Append(rect.DOAnchorPosX(-1920f, 1f));
    }
}
