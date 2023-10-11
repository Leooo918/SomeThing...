using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Effect : MonoBehaviour
{
    protected PlayerStatus playerStatus = null;
    protected float effectDuration = 0f;
    protected float originValue = 0f;

    protected virtual void Awake()
    {
        playerStatus = GetComponentInParent<PlayerStatus>();
    }

    protected abstract void EffectEffort();
}
