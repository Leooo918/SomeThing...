using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FallDownAnimation : CharacterAnimation
{
    private Sequence seq = null;

    public override void Animation()
    {
        seq = DOTween.Sequence();

        seq.Append(rect.DORotate(new Vector3(0, 0, -60), 0.3f))
            .Join(rect.DOMoveY(-500,0.3f));
    }
}
