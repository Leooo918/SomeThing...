using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string itemName = "";

    protected PlayerInput playerAction = null;
    protected PlayerStatus playerStatus = null;
    protected WeaponSO weaponSO = null;
    protected WeaponStruct weaponStruct = new WeaponStruct();

    protected float originDamage = 0f;
    protected float damage = 0f;

    protected float attackCoolTime = 0f;
    protected float skillCoolTime = 0f;
    protected float curSkillCoolDown = 0f;

    protected float proficiency = 0f;       //숙련도
    protected float durability = 100f;      //내구도

    protected int proficiencyLv = 0;
    protected string weaponExplain = "";

    protected bool isAttackCool = false;
    protected bool isSkillCool = false;
    protected bool isUsingSkill = false;
    protected bool isAttaking = false;

    public string ItemName => itemName;
    public float Durability => durability;

    protected virtual void Awake()
    {
        playerAction = GetComponentInParent<PlayerInput>();
        playerStatus = playerAction.GetComponent<PlayerStatus>();
    }

    protected virtual void Start()
    {
        weaponSO = GameManager.instance.weaponSO;

        for (int i = 0; i < weaponSO.weapons.Length; i++)
        {
            if (weaponSO.weapons[i].name == itemName)
            {
                weaponStruct = weaponSO.weapons[i];
                damage = weaponStruct.weaponStatus.damage[proficiencyLv];
                originDamage = damage;
                attackCoolTime = weaponStruct.weaponStatus.normalAttackCool;
                skillCoolTime = weaponStruct.weaponStatus.skillCool[proficiencyLv];
                print(skillCoolTime);
                print(attackCoolTime);
            }
        }
    }

    protected virtual void Update()
    {
        if (curSkillCoolDown > 0)
        {
            curSkillCoolDown -= Time.deltaTime;
        }
        else
        {
            isSkillCool = false;
        }
    }

    protected virtual void OnEnable()
    {
        playerAction.onAttackButtonPress = OnAttack;
        playerAction.onUseSkill = OnUseSkill;
    }

    protected virtual void OnDisable()
    {
        if (playerAction != null)
        {
            playerAction.onAttackButtonPress -= OnAttack;
            playerAction.onUseSkill -= OnUseSkill;
        }
    }

    protected virtual void OnUseSkill()
    {
        ProficiencyUp();
        DurabilityDown();
    }
    protected virtual void OnAttack()
    {
        ProficiencyUp();
        DurabilityDown();
    }

    protected void AttackCoolDown()
    {
        StartCoroutine("AttackCoolRoutine");
    }

    protected void SKillCoolDown()
    {
        curSkillCoolDown = skillCoolTime;
        isSkillCool = true;
    }

    IEnumerator AttackCoolRoutine()
    {
        isAttackCool = true;
        yield return new WaitForSeconds(attackCoolTime);
        isAttackCool = false;
    }

    public virtual void ProficiencyUp()
    {
        if (proficiencyLv == 4) return;

        if (Random.Range(0, 10) < 3)
        {
            proficiency += 0.1f / (proficiencyLv + 1f);

            if (proficiency >= 100f)
            {
                proficiencyLv++;
                proficiency -= 100f;
                originDamage = weaponStruct.weaponStatus.damage[proficiencyLv];
                damage = originDamage;
            }
            WeaponManager.instance.JsonSave(itemName, proficiencyLv, proficiency);
        }
    }

    public virtual void DurabilityDown()
    {
        if (Random.Range(0, 2) < 1) //50%확률로 내구도가 0.1에서 0.9까지 줄어듬(내구도는 최대 100, 수리 가능)
        {
            float value = Random.Range(1, 6) / 10f;
            durability -= value;
        }
    }

    public void Init(float durability)
    {
        this.durability = durability;
        this.proficiency = WeaponManager.instance.GetWeaponProficiencyValue(itemName);
        this.proficiencyLv = WeaponManager.instance.GetWeaponProficiencyLv(itemName);
    }
}