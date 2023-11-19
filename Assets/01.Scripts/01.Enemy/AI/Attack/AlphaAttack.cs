using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaAttack : AIAttack
{
    [SerializeField] private GameObject attackWarning;
    [SerializeField] private GameObject skillWarning;
    private EnemyBrain brain;
    private DamageSource damage ;
    private Animator anim;
    private EnemyMove move;

    private float attackDelay = 0.5f;
    private float skillDelay = 2f;

    private Vector3 attackDir;
    private Vector3 skillDir;


    protected override void Awake()
    {
        base.Awake();
        move = GetComponent<EnemyMove>();
        brain = GetComponent<EnemyBrain>();
        damage = transform.Find("Dash"). GetComponent<DamageSource>();

        damage.damage = status.attack;
        damage.gameObject.SetActive(false);

        damage.onAttack += (Collider2D c) =>
        {
            if(c. TryGetComponent<PlayerMove>(out PlayerMove p))
            {
                Debug.Log(skillDir);
                p.KnockLong(skillDir, 20f, 0.2f);
            }

            if(c.TryGetComponent<EnemyMove>(out EnemyMove e))
            {
                Debug.Log(skillDir);
                e.Dash(skillDir, 20f, 0.2f);
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (attackCoolDown > 0 ) canAttack = false;
        else canAttack = true;

        if (skillCoolDown > 0 ) canUseSkill = false;
        else canUseSkill = true;
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
        damage.gameObject.SetActive(true);
        move.Dash(skillDir, 0.5f,12);
        yield return new WaitForSeconds(0.5f);
        damage.gameObject.SetActive (false);
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
