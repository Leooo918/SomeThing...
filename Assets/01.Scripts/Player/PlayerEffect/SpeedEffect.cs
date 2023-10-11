using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEffect : Effect
{
    private float value = 0f;

    protected override void EffectEffort()
    {
        StartCoroutine("Effort");
    }

    IEnumerator Effort()
    {
        originValue = playerStatus.PlayerSpeed;
        playerStatus.ChangeSpeed(value);
        yield return new WaitForSeconds(effectDuration);
        playerStatus.ChangeSpeed(originValue);
    }
}
