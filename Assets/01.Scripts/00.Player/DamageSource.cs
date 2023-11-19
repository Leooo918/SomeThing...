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
            if(GetComponentInParent<PlayerStatus>() != null)
            {
                CameraManager.instance.ShakeCam(5, 1, 0.1f);                    //카메라 흔들어
            }
            Debug.Log(damage * damageMultiple);
            Debug.Log(collision.name);
            damageAble.Damaged(damage * damageMultiple, transform.position);
            onAttack?.Invoke(collision);
        }
    }
}
