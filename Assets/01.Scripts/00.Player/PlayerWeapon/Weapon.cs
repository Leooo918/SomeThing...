using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string itemName = "";

    protected PlayerInput playerAction = null;
    protected PlayerStatus playerStatus = null;
    protected WeaponSO weaponSO = null;
    protected PlayerMove playerMove = null;
    protected WeaponParents parent = null;
    protected WeaponStruct weaponStruct = new WeaponStruct();

    protected float originDamage = 0f;
    protected float damage = 0f;

    protected float attackCoolTime = 0f;
    protected float skillCoolTime = 0f;
    protected float subSkillCoolTime = 0f;

    protected float curSkillCoolDown = 0f;
    protected float curSubSkillCoolTime = 0f;

    protected float proficiency = 0f;       //숙련도
    protected float durability = 100f;      //내구도

    protected int proficiencyLv = 0;
    protected string weaponExplain = "";

    protected bool isAttackCool = false;
    protected bool isSkillCool = false;
    protected bool isSubSkillCool = false;
    protected bool isUsingSkill = false;
    protected bool isAttaking = false;

    protected Image mainSkillCool = null;
    protected Image subSkillCool = null;
    protected TextMeshProUGUI mainSkillCoolTxt = null;
    protected TextMeshProUGUI subSkillCoolTxt = null;

    public string ItemName => itemName;
    public float Durability => durability;

    protected virtual void Awake()
    {
        playerAction = GetComponentInParent<PlayerInput>();
        playerMove = GetComponentInParent<PlayerMove>();
        playerStatus = GetComponentInParent<PlayerStatus>();
        parent = GetComponentInParent<WeaponParents>();
    }

    protected virtual void OnEnable()
    {
        if (playerAction == null)
        {
            if (gameObject.activeSelf == true)
            {
                Destroy(gameObject);
            }
            return;
        }


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
                subSkillCoolTime = weaponStruct.weaponStatus.subSkillCool;
            }
        }

        

        parent.WeaponDirByMouse();
    }

    protected virtual void Update()
    {
        if (curSkillCoolDown > 0)
        {
            mainSkillCool.fillAmount = curSkillCoolDown / skillCoolTime;
            mainSkillCoolTxt.SetText(((int)curSkillCoolDown).ToString());
            curSkillCoolDown -= Time.deltaTime;
        }
        else
        {
            isSkillCool = false;
            if (mainSkillCoolTxt != null)
            {
                mainSkillCoolTxt.gameObject.SetActive(false);
            }
        }

        if (curSubSkillCoolTime > 0)
        {
            subSkillCool.fillAmount = curSubSkillCoolTime / subSkillCoolTime;
            subSkillCoolTxt.SetText(((int)curSubSkillCoolTime).ToString());
            curSubSkillCoolTime -= Time.deltaTime;
        }
        else
        {
            isSubSkillCool = false;
            if (subSkillCoolTxt != null)
            {
                subSkillCoolTxt.gameObject.SetActive(false);
            }
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

    protected abstract void OnUseSubSkill();

    protected void AttackCoolDown()
    {
        StartCoroutine("AttackCoolRoutine");
    }

    protected void SkillCoolDown()
    {
        mainSkillCoolTxt.gameObject.SetActive(true);
        curSkillCoolDown = skillCoolTime;
        isSkillCool = true;
    }
    protected void SubSkillCoolDown()
    {
        subSkillCoolTxt.gameObject.SetActive(true);
        curSubSkillCoolTime = subSkillCoolTime;
        isSubSkillCool = true;
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

    public void SetSkill()
    {
        UIManager.instance.SetPlayerSkill(durability, weaponStruct.weaponImageObj, weaponStruct.weaponSkillIcon, weaponStruct.weaponSubSkillIcon);

        mainSkillCool = UIManager.instance.MainSkillCool;
        subSkillCool = UIManager.instance.SubSkillCool;

        mainSkillCoolTxt = mainSkillCool.transform.Find("CoolTime").GetComponent<TextMeshProUGUI>();
        subSkillCoolTxt = subSkillCool.transform.Find("CoolTime").GetComponent<TextMeshProUGUI>();
    }


    public void Init(float durability)
    {
        if (GetComponentInParent<PlayerInput>() == null && gameObject.activeSelf == true)
        {
            Destroy(gameObject);
            return;
        }
        this.durability = durability;
        this.proficiency = WeaponManager.instance.GetWeaponProficiencyValue(itemName);
        this.proficiencyLv = WeaponManager.instance.GetWeaponProficiencyLv(itemName);
    }
}