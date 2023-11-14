using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerViewDecision : AIDecision
{
    [SerializeField] private float viewDistance = 7f;

    public override bool MakeDesition()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, viewDistance, LayerMask.GetMask("Player"));

        if (coll != null) return true;
        return false;
    }
}
