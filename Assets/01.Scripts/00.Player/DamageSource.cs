using System;
using System.Collections.Generic;
using UnityEngine;


public class DamageSource : MonoBehaviour
{
    public float damage = 0;
    public float damageMultiple = 1;

    public Action<Collider2D> onAttack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamageable>(out IDamageable damageAble))
        {
            Debug.Log(damage * damageMultiple);
            Debug.Log(collision.name);
            onAttack?.Invoke(collision);
            damageAble.Damaged(damage * damageMultiple);
        }
    }
}
