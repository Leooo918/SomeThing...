using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAttack : MonoBehaviour
{
    protected EnemyStatus status;

    protected float attackCoolTime;
    public float attackCoolDown { get; protected set; }

    protected float skillCoolTime;
    public float skillCoolDown { get; protected set; }

    public bool canAttack { get; protected set; }
    public bool canUseSkill { get; protected set; }

    public bool isAttacking { get; protected set; }
    public bool isUsingSkill { get; protected set; }


    protected virtual void Awake()
    {
        status = GetComponent<EnemyStatus>();
    }

    protected virtual void Update()
    {
        if (attackCoolDown > 0)
        {
            attackCoolDown -= Time.deltaTime;
        }
        if (skillCoolDown > 0)
        {
            skillCoolDown -= Time.deltaTime;
        }
    }

    public abstract void Attack(Vector3 target);

    public abstract void CancelAttack();

    public abstract void Skill(Vector3 target);

    public abstract void CancelSkill();

    public virtual void SetCoolTime(float attackCool, float skillCool)
    {
        attackCoolTime = attackCool;
        skillCoolTime = skillCool;
    }
}
