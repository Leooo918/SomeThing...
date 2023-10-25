using UnityEngine;
using System.Collections;

public class PlayerKatana : Weapon
{
    private DamageSource damageSource = null;
    private KatanaSwordHolder swordHolder = null;
    private SkillCheckFeedBack skillCheckFeedBack = null;

    private Animator attackAnimator = null;
    private Collider2D attackColl = null;
    private Transform katanaTrm = null;
    [SerializeField] private GameObject swordStrike = null;

    private bool readyGetSkillCheckResult = false;
    private bool readyUseSkill = false;

    protected override void Awake()
    {
        base.Awake();

        damageSource = GetComponentInChildren<DamageSource>();
        swordHolder = transform.Find("SwordParents").GetComponent<KatanaSwordHolder>();
        skillCheckFeedBack = GetComponent<SkillCheckFeedBack>();
        attackAnimator = GetComponent<Animator>();
        katanaTrm = transform.Find("SwordParents/Sword");
        attackColl = katanaTrm.GetComponent<BoxCollider2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
        damageSource.damage = damage;
    }

    protected override void Update()
    {
        base.Update();

        if (readyGetSkillCheckResult == true && Input.GetKeyDown(KeyCode.Space))
        {
            switch (skillCheckFeedBack.EndSkillCheck())
            {
                case SkillCheckResult.BigSuccess:
                    damageSource.damageMultiple = 10f;
                    break;
                case SkillCheckResult.Success:
                    damageSource.damageMultiple = 5f;
                    break;
                case SkillCheckResult.Fail:
                    break;
            }
            readyUseSkill = true;
        }
    }

    protected override void OnAttack()
    {
        if (isUsingSkill == true)
        {
            skillCheckFeedBack.EndSkillCheck();
            StartCoroutine(BooleanAnimation("CancelSkill"));
            curSkillCoolDown = skillCoolTime / 2;
            isUsingSkill = false;
            readyUseSkill = false;
        }

        if (isAttackCool == true || isUsingSkill == true || isAttaking == true) 
        {
            return;
        } 

        base.OnAttack();

        StartCoroutine(BooleanAnimation("Attack"));
    }

    protected override void OnUseSkill()
    {
        if (proficiencyLv < 1 || isSkillCool == true || isAttaking == true)
        {
            return;
        }

        base.OnUseSkill();

        swordHolder.UnMouseFollow();

        if (readyUseSkill == false)
        {
            if (isUsingSkill == true) return;
            isUsingSkill = true;
            StartCoroutine(BooleanAnimation("SkillStart"));
        }
        else
        {
            StartCoroutine(BooleanAnimation("UseSkill"));
        }

    }

    public void OnReadyEnd()
    {
        swordHolder.MouseFollow();
    }

    public void SkillCheck()
    {
        skillCheckFeedBack.StartSkillCheck();
        readyGetSkillCheckResult = true;
    }

    public void UseSkill()
    {
        swordStrike.SetActive(true);
        //swordStrike.GetComponent<SwordStrike>().SetDir();
        swordStrike.transform.position = transform.position;
        swordStrike.GetComponent<DamageSource>().damage = damage * damageSource.damageMultiple;

        damage = originDamage;
        damageSource.damageMultiple = 1f;


        swordHolder.MouseFollow();
        readyUseSkill = false;
        isUsingSkill = false;
        SKillCoolDown();
        OnAttackEnd();
    }

    public void OnAttackStart()
    {
        attackColl.enabled = true;
        isAttaking = true;
    }
    public void OnAttackEnd()
    {
        attackColl.enabled = false;

        isAttaking = false;
        swordHolder.MouseFollow();
        AttackCoolDown();
    }

    IEnumerator BooleanAnimation(string parmName)
    {
        print(parmName);
        attackAnimator.SetBool(parmName, true);
        yield return null;
        yield return null;
        yield return null;
        attackAnimator.SetBool(parmName, false);
    }

}
