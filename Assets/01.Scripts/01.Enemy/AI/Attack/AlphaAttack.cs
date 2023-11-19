using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaAttack : AIAttack
{
    [SerializeField] private GameObject attackWarning;
    [SerializeField] private GameObject skillWarning;
    private EnemyBrain brain = null;
    private Animator anim;

    private float attackDelay = 0.5f;
    private float skillDelay = 2f;

    private Vector3 attackDir;
    private Vector3 skillDir;


    protected override void Awake()
    {
        base.Awake();
        brain = GetComponent<EnemyBrain>();
    }

    public override void Attack(Vector3 target)
    { 
        if (canAttack == false || isAttacking) return;

        attackDir = (target - transform.position).normalized;

        attackWarning.SetActive(true);
        attackWarning.transform.position = transform.position + attackDir * 1f;
        attackWarning.GetComponent<EnemyWarning>().StartWarning(attackDelay);
        StartCoroutine("StartAttack");
    }

    protected override void Update()
    {
        base.Update();

        if (attackCoolDown > 0 ) canAttack = false;
        else canAttack = true;

        if (skillCoolDown > 0 ) canUseSkill = false;
        else canUseSkill = true;
    }

    IEnumerator StartAttack()
    {
        brain.Stop();
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        //anim.SetTrigger("Attack");
        Collider2D colls = Physics2D.OverlapCircle(transform.position + attackDir * 1f, 0.5f, LayerMask.GetMask("Player"));
       
        if (colls != null)  colls.GetComponent<PlayerStatus>().Damaged(status.attack, transform.position);
        attackCoolDown = attackCoolTime;
        yield return new WaitForSeconds(0.1f);
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
        if (canUseSkill == false || isUsingSkill) return;

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
        brain.Stop();
        isUsingSkill = true;
        yield return new WaitForSeconds(skillDelay);
        skillCoolDown = skillCoolTime;
        yield return new WaitForSeconds(0.1f);
        skillCoolDown = skillCoolTime;
        isUsingSkill = false;
    }

    public override void CancelSkill()
    {
        isUsingSkill = false;
    }
}
