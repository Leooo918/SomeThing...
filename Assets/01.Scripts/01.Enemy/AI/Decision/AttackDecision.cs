using UnityEngine;

public enum AttackKind
{
    Attack = 0,
    Skill = 2
}

public class AttackDecision : AIDecision
{
    [SerializeField] private float attackRange = 4;
    [SerializeField] AttackKind skill = AttackKind.Attack;
    public override bool MakeDesition()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));

        AIAttack attack = _enemyBrain.GetComponent<AIAttack>();

        if (coll != null)
        {
            if (attack.canAttack && skill == AttackKind.Attack) return true;

            if (attack.canUseSkill && skill == AttackKind.Skill) return true;
        }
        if (attack.isAttacking == false && attack.isUsingSkill == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}