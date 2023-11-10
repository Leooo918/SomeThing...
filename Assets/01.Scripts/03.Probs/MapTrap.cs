using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrap : MonoBehaviour
{
    private Collider2D coll = null;
    private Animator anim = null;

    private bool isActivate = false;

    [SerializeField] private bool isActiveOnTime = true;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float activeCoolTime = 1f;
    private float activeCoolTimeDown = 0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (activeCoolTimeDown > 0)
        {
            activeCoolTimeDown -= Time.deltaTime;
        }

        if (isActiveOnTime == false)
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 0.5f);

            foreach (Collider2D coll in colls)
            {
                if (activeCoolTimeDown <= 0 && coll.TryGetComponent<Rigidbody2D>(out Rigidbody2D r) == true)
                {
                    print(coll.name);
                    ActiveTrap();
                }
            }
        }
        else
        {
            if (activeCoolTimeDown <= 0 && isActivate == false)
            {
                print("¾ßÈ£");
                ActiveTrap();
            }
        }
    }

    public void ActiveTrap()
    {
        if (activeCoolTimeDown <= 0)
        {
            anim.SetTrigger("Active");
            activeCoolTimeDown = activeCoolTime;
            anim.SetTrigger("Disabled");
        }
    }

    public void DisableTrap()
    {
        activeCoolTimeDown = activeCoolTime;
        anim.SetTrigger("Disabled");
    }

    public void ColliderOn()
    {
        coll.enabled = true;
        isActivate = true;
    }

    public void ColliderOff()
    {
        coll.enabled = false;
        isActivate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamageable>(out IDamageable i))
        {
            i.Damaged(damage, transform.position);
        }
    }

}
