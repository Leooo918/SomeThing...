using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerDagger : Weapon
{
    private Sequence seq = null;

    private DamageSource damageSource = null;
    private PlayerStatus status = null;
    private Collider2D coll = null;
    private Transform daggerTrm = null;
    private Vector2 dir;



    protected override void Awake()
    {
        base.Awake();

        damageSource = GetComponent<DamageSource>();
        status = GetComponentInParent<PlayerStatus>();
        coll = GetComponent<Collider2D>();
        daggerTrm = transform.Find("Sprite");
        coll.enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerAction.onAttackButtonPress += OnAttack;
        playerAction.onUseSkill += OnUseSkill;
        playerAction.onUseSubSkill += OnUseSubSkill;
        playerAction.onMouseMove += PlayerDir;
    }

    protected void OnDisable()
    {
        if (playerAction != null)
        {
            playerAction.onAttackButtonPress -= OnAttack;
            playerAction.onUseSkill -= OnUseSkill;
            playerAction.onUseSubSkill -= OnUseSubSkill;
            playerAction.onMouseMove -= PlayerDir;
            damageSource.onAttack -= OnSkillHit;
        }
    }

    protected void Start()
    {
        damageSource.damage = damage;
        damageSource.damageMultiple = 1;
    }

    protected override void Update()
    {
        base.Update();
        if (isAttaking == false || isUsingSkill == false)
        {
            transform.position = playerStatus.transform.position;
        }
    }

    protected override void OnAttack()
    {
        if (isAttackCool == true || isAttaking == true)
        {
            return;
        }

        base.OnAttack();

        AttackOnece();

        AttackCoolDown();
    }

    protected override void OnUseSkill()
    {
        if (proficiencyLv < 1 || isSkillCool == true || isAttaking == true || isUsingSkill == true)
        {
            return;
        }

        base.OnUseSkill();

        UseSkill();

        SkillCoolDown();
    }

    protected override void OnUseSubSkill()
    {
        if (proficiencyLv < 4 || isSubSkillCool == true)
        {
            return;
        }

        playerMove.Flash(2);

        SubSkillCoolDown();
    }

    public void UseSkill()
    {
        seq = DOTween.Sequence();

        playerAction.onMouseMove -= PlayerDir;
        damageSource.onAttack += OnSkillHit;

        playerMove.canNotMove = true;

        for (int i = 0; i < 5; i++)
        {
            seq.Append(transform.DOMove((Vector2)status.transform.position + dir.normalized * 0.5f, 0.1f))
                .Append(transform.DOMove(status.transform.position, 0.01f));
        }

        seq.OnStart(() => coll.enabled = true)
            .OnComplete(() =>
            {
                coll.enabled = false;
                damageSource.damageMultiple = 1;
                damageSource.onAttack -= OnSkillHit;
                playerAction.onMouseMove += PlayerDir;
                playerMove.canNotMove = false;
            });

    }

    public void OnSkillHit(Collider2D collision)
    {
        damageSource.damageMultiple += 0.2f; //
    }

    public void OnSkillEnd()
    {
        damageSource.damageMultiple = 1f;   //
    }

    private void PlayerDir(Vector2 dir)
    {
        this.dir = dir;
        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float rotation = Mathf.LerpAngle(transform.eulerAngles.z, rot, 1f);
        transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    private void AttackOnece()
    {
        seq = DOTween.Sequence();

        seq.Append(transform.DOMove((Vector2)status.transform.position + dir.normalized * 0.5f, 0.1f))
            .Append(transform.DOMove(status.transform.position, 0.01f))
            .OnStart(() => coll.enabled = true)
            .OnComplete(() => coll.enabled = false);
    }
}
