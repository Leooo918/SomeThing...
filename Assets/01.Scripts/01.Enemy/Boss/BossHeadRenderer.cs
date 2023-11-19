using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadRenderer : MonoBehaviour
{
    private Animator anim = null;
    private SpriteRenderer sr = null;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetAnimation(string param)
    {
        anim.SetTrigger(param);
    }
}
