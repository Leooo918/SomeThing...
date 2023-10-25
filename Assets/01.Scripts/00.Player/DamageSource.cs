using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageSource : MonoBehaviour
{
    public float damage = 0;
    public float damageMultiple = 1;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamageable>(out IDamageable damageAble))
        {
            Debug.Log(damage * damageMultiple);
            damageAble.Damaged(damage * damageMultiple);
        }
    }
}
