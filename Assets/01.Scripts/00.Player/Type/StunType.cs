using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunType : Type
{
    private DamageSource damageSource;
    public float stunTime = 1f;


    private void Awake()
    {
        damageSource = GetComponent<DamageSource>();

        damageSource.onAttack += Effect;
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    protected override void Effect(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyStatus>(out EnemyStatus enemyStat))
        {
            enemyStat.Stun(stunTime);
        }
    }
}
