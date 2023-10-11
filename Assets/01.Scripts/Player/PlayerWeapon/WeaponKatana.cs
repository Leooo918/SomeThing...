using UnityEngine;

public class PlayerKatana : Weapon
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnAttack()
    {
        ProficiencyUp(0.5f);                    //이거 값은 나중에 SO에서 빼오는 걸로 바꿔!!!!!!!
        DurabilityDown();


    }

    protected override void OnUseSkill()
    {
        if (proficiency < 10) return;   //숙련도 10이상이면 사용가능 기본공격 20번 사용시 가능

        ProficiencyUp(1f);                    //이거 값은 나중에 SO에서 빼오는 걸로 바꿔!!!!!!!
        DurabilityDown();


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Damaged(playerStatus.PlayerAttact * damageMultiple);
        }
    }
}
