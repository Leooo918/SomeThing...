using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadAttack : AIAttack
{
    private BossHeadRenderer rend;

    protected override void Awake()
    {
        base.Awake();
        rend = transform.Find("Sprite").GetComponent<BossHeadRenderer>();
    }

    public override void Attack(Vector3 target)
    {
        throw new System.NotImplementedException();
    }

    public override void CancelAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void Skill(Vector3 target)
    {
        throw new System.NotImplementedException();
    }

    public override void CancelSkill()
    {
        throw new System.NotImplementedException();
    }

    public void BossAttack()
    {

    }
}
