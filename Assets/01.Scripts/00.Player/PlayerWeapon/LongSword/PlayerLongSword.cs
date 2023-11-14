using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLongSword : Weapon
{
    private Animator anim = null;
    private PlayerInput playerInput = null;


    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        playerInput = GetComponentInParent<PlayerInput>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerInput.onMove += CheckMove;
        parent.WeaponDirByMove();
    }

    protected void OnDisable()
    {
        playerInput.onMove -= CheckMove;
    }

    private void CheckMove(Vector2 v)
    {
        if (v.magnitude > 0.2f) anim.SetBool("Move", true);
        else anim.SetBool("Move", false);

        if(v.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
        }
        else if(v.y < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
        }
    }

    protected override void OnUseSubSkill()
    {
        if (proficiencyLv < 4 || isSubSkillCool == true)
        {
            return;
        }
    }

    protected override void OnAttack()
    {
        if (isAttackCool == true || isUsingSkill == true || isAttaking == true)
        {
            return;
        }
        base.OnAttack();

        BooleanAnimation("Attack");
    }

    protected override void OnUseSkill()
    {
        if (proficiencyLv < 1 || isSkillCool == true || isAttaking == true)
        {
            return;
        }
        else if (isUsingSkill == true)
        {
            return;
        }
        base.OnUseSkill();

        BooleanAnimation("Skill");
    }


    IEnumerator BooleanAnimation(string parmName)
    {
        anim.SetBool(parmName, true);
        yield return null;
        yield return null;
        yield return null;
        anim.SetBool(parmName, false);
    }
}
