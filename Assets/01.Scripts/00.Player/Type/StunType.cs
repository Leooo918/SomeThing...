using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunType : Type
{
    public float stunTime = 1f;

    protected override void Effect()
    {

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<EnemyStatus>(out EnemyStatus enemyStat))
        {
            enemyStat.Stun(stunTime);
        }
    }
}
