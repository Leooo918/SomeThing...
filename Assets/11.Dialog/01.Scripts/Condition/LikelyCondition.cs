using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Charactor
{
    Amelia = 0,
    Suba = 1
}

public class LikelyCondition : Condition
{
    private PlayerDialogStatus status = null;

    [SerializeField] private Charactor charactor;
    [SerializeField] private float leastLikelyValue;

    public override bool Judge()
    {
        status = DialogGameManager.instance.Status;

        if (charactor == Charactor.Amelia)
        {
            if(status.firstLikeability >= leastLikelyValue)
            {
                return true;
            }
        }
        else if(charactor == Charactor.Suba)
        {
            if (status.secondLikeablity >= leastLikelyValue)
            {
                return true;
            }
        }
        return false;
    }
}
