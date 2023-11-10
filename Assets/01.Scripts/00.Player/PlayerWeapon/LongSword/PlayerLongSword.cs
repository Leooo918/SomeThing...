using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLongSword : Weapon
{
    private Animator anim = null;


    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
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
