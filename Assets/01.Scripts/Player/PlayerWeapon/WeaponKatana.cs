using UnityEngine;

public class PlayerKatana : Weapon
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnAttack()
    {
        ProficiencyUp(0.5f);                    //�̰� ���� ���߿� SO���� ������ �ɷ� �ٲ�!!!!!!!
        DurabilityDown();


    }

    protected override void OnUseSkill()
    {
        if (proficiency < 10) return;   //���õ� 10�̻��̸� ��밡�� �⺻���� 20�� ���� ����

        ProficiencyUp(1f);                    //�̰� ���� ���߿� SO���� ������ �ɷ� �ٲ�!!!!!!!
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
