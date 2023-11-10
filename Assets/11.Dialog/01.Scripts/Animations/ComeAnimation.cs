using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ComeAnimation : CharacterAnimation
{


    public override void Animation()
    {
        rect.DOAnchorPosX(0, 1f);
    }
}
