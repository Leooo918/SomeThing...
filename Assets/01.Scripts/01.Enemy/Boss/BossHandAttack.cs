using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandAttack : AIAttack
{
    private EnemyBrain brain;
    private BossHandRenderer render;
    private GameObject attackRange;
    private GameObject skillRange;
    private Rigidbody2D rb;

    private Vector2 originPos;

    private bool isMoving = false;
    private bool isSweeping = false;

    private bool isAttacking = false;
    private bool isUsingSkill = false;

    protected override void Awake()
    {
        base.Awake();
        brain = GetComponent<EnemyBrain>();
        status = GetComponent<EnemyStatus>();
        render = transform.Find("Sprite").GetComponent<BossHandRenderer>();

        attackRange = transform.Find("AttackHitPoint").gameObject;
        attackRange.SetActive(false);
        skillRange = transform.Find("SkillHitPoint").gameObject;
        skillRange.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (isMoving == true)
        {
            rb.velocity = (transform.position - brain.playerTrm.position).normalized * status.speed;
        }

        if (isSweeping == true)
        {
            switch (status.enemyName)
            {
                case "LeftHand":
                    rb.velocity = new Vector2(1, 0) * status.speed * 2;
                    break;
                case "RightHand":
                    rb.velocity = new Vector2(-1, 0) * status.speed * 2;
                    break;
            }
            rb.velocity = new Vector2(1, 0);
        }
    }

    public override void Attack(Vector3 target)
    {
        if (isAttacking) return;
        isAttacking = true;
        StartCoroutine("AttackRoutine");
    }

    IEnumerator AttackRoutine()
    {
        isMoving = true;
        yield return new WaitForSeconds(1f);
        isMoving = false;
        render.SetAttackAnim();
    }

    public override void Skill(Vector3 target)
    {
        if (isUsingSkill) return;
        isUsingSkill = true;
        StartCoroutine("SkillRoutine");
    }

    IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(1f);
        render.SetSkillAnim();
    }


    public void BossSkill()
    {

    }

    public void StartAttack()
    {
        attackRange.SetActive(true);
    }
    public void EndAttack()
    {
        attackRange.SetActive(false);
    }
    public void StartUseSkill()
    {
        skillRange.SetActive(true);
    }
    public void EndUseSkill()
    {
        skillRange.SetActive(false);
    }


    public override void CancelAttack()
    {
    }

    public override void CancelSkill()
    {
    }
}
