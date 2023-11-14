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
    private StunType stunType = null;
    [SerializeField] private GameObject swordStrike = null;

    private bool readyGetSkillCheckResult = false;
    private bool readyUseSkill = false;

    private float originSpeed = 0f;

    protected override void Awake()
    {
        base.Awake();

        damageSource = GetComponentInChildren<DamageSource>();
        swordHolder = transform.Find("SwordParents").GetComponent<KatanaSwordHolder>();
        skillCheckFeedBack = GetComponent<SkillCheckFeedBack>();
        attackAnimator = GetComponent<Animator>();
        katanaTrm = transform.Find("SwordParents/Sword");
        attackColl = katanaTrm.GetComponent<BoxCollider2D>();
        stunType = katanaTrm.GetComponent<StunType>();
    }


    protected void OnDisable()
    {
        if (playerAction != null)
        {
            playerAction.onAttackButtonPress -= OnAttack;
            playerAction.onUseSkill -= OnUseSkill;
            playerAction.onUseSubSkill -= OnUseSubSkill;
        }
    }

    protected void Start()
    {
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
            skillCoolTime /= 2;
            SkillCoolDown();
            skillCoolTime *= 2;
            isUsingSkill = false;
            readyUseSkill = false;
            playerStatus.ChangeSpeed(originSpeed);
            damageSource.damageMultiple = 1f;
        }

        if (isAttackCool == true || isUsingSkill == true || isAttaking == true) 
        {
            return;
        }   

        base.OnAttack();

        playerMove.canNotMove = true;
        StartCoroutine(BooleanAnimation("Attack"));
    }

    protected override void OnUseSkill()
    {
        if (proficiencyLv < 1 || isSkillCool == true || isAttaking == true)
        {
            return;
        }
        else if(readyUseSkill == false && isUsingSkill == true)
        {
            return;
        }

        base.OnUseSkill();

        swordHolder.UnMouseFollow();

        if (readyUseSkill == false)
        {
            mainSkillCool.fillAmount = 1f;
            isUsingSkill = true;
            originSpeed = playerStatus.PlayerSpeed;
            StartCoroutine(BooleanAnimation("SkillStart"));
            playerStatus.ChangeSpeed(playerStatus.PlayerSpeed - 3f);
        }
        else
        {
            StartCoroutine(BooleanAnimation("UseSkill"));
        }

    }

    public void OnReadyEnd()
    {
        swordHolder.MouseFollow();

        playerAction.onAttackButtonPress += OnAttack;
        playerAction.onUseSkill += OnUseSkill;
        playerAction.onUseSubSkill += OnUseSubSkill;
    }

    public void SkillCheck()
    {
        if(isUsingSkill == false)
        {
            return;
        }
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
        SkillCoolDown();
        OnAttackEnd();
    }

    public void OnStartUseSkill()
    {
        stunType.enabled = true;
        playerStatus.ChangeSpeed(originSpeed);
        playerMove.Dash(0.05f, 10);
    }

    public void OnAttackStart()
    {
        attackColl.enabled = true;
        isAttaking = true;
    }
    public void OnAttackEnd()
    {
        stunType.enabled = false;
        attackColl.enabled = false;
        playerMove.canNotMove = false;
        isAttaking = false;
        swordHolder.MouseFollow();
        AttackCoolDown();
    }

    IEnumerator BooleanAnimation(string parmName)
    {
        attackAnimator.SetBool(parmName, true);
        yield return null;
        yield return null;
        yield return null;
        attackAnimator.SetBool(parmName, false);
    }

    protected override void OnUseSubSkill()
    {
        if (proficiencyLv < 4 || isSubSkillCool == true)
        {
            return;
        }

        playerMove.Flash(3.5f);

        SubSkillCoolDown();
    }
}
