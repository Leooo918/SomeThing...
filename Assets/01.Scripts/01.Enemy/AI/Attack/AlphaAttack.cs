using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaAttack : AIAttack
{
    [SerializeField] private GameObject attackWarning;
    [SerializeField] private GameObject skillWarning;
    private Animator anim;

    private float attackDelay = 0.5f;
    private float skillDelay = 2f;

    private Vector3 attackDir;
    private Vector3 skillDir;

    private bool isAttacking;
    private bool isUsingSkill;

    public override void Attack(Vector3 target)
    { 
        if (isAttacking == true && attackCoolDown <= 0) return;

        attackDir = (target - transform.position).normalized;

        attackWarning.SetActive(true);
        attackWarning.transform.position = transform.position + attackDir * 1f;
        attackWarning.GetComponent<EnemyWarning>().StartWarning(attackDelay);
        StartCoroutine("StartAttack");
    }

    protected override void Update()
    {
        base.Update();

        if (attackCoolDown > 0 || isAttacking) canAttack = false;
        else canAttack = true;

        if (skillCoolDown > 0 || isUsingSkill) canUseSkill = false;
        else canUseSkill = true;
    }

    IEnumerator StartAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        //anim.SetTrigger("Attack");
        Collider2D colls = Physics2D.OverlapCircle(transform.position + attackDir * 1f, 0.5f, LayerMask.GetMask("Player"));
       
        if (colls != null)  colls.GetComponent<PlayerStatus>().Damaged(status.attack, transform.position);
        
        attackCoolDown = attackCoolTime;
        isAttacking = false;
    }

    public override void CancelAttack()
    {
        isAttacking = false;
        StopCoroutine("StartAttack");
        attackWarning.GetComponent<EnemyWarning>().StopWarning();
    }

    public override void Skill(Vector3 target)
    {
        if (isUsingSkill) return;

        skillDir = (target - transform.position).normalized;
        float angle = Mathf.Atan2(skillDir.y, skillDir.x) * Mathf.Rad2Deg;

        skillWarning.transform.up =  skillDir;
        skillWarning.SetActive(true);
        skillWarning.transform.position = transform.position + skillDir * 3f;
        skillWarning.GetComponent<EnemyWarning>().StartWarning(skillDelay);
        StartCoroutine("StartSkill");
    }

    IEnumerator StartSkill()
    {
        isUsingSkill = true;
        yield return new WaitForSeconds(skillDelay);
        skillCoolDown = skillCoolTime;
        //Collider2D colls = Physics2D.Overlap(transform.position, 0.5f, LayerMask.GetMask("Player"));

        //if (colls != null)
        //{
        //    colls.GetComponent<PlayerStatus>().Damaged(status.attack, transform.position);
        //}
        isUsingSkill = false;
    }

    public override void CancelSkill()
    {
        isUsingSkill = false;
    }
}