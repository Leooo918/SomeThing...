using UnityEngine;
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string itemName = "";
    protected WeaponSave weaponData = new WeaponSave();
    protected PlayerInput playerAction = null;
    protected PlayerStatus playerStatus = null;
    protected WeaponSO weaponSO = null;
    protected float damageMultiple = 0f;
    protected float proficiency = 0f;       //숙련도
    protected float durability = 100f;      //내구도

    public string ItemName => itemName;
    public WeaponSave WeaponData => weaponData;

    protected virtual void Awake()
    {
        playerAction = GetComponentInParent<PlayerInput>();
        playerStatus = playerAction.GetComponent<PlayerStatus>();

        if (PlayerPrefs.HasKey(itemName + "Proficiency"))
            PlayerPrefs.SetFloat(itemName + "Proficiency", proficiency);

        if (PlayerPrefs.HasKey(itemName + "Durability"))
            PlayerPrefs.SetFloat(itemName + "Durability", durability);

        proficiency = PlayerPrefs.GetFloat(itemName + "Proficiency");
        durability = PlayerPrefs.GetFloat(itemName + "Durability");
    }

    protected virtual void Start()
    {
        weaponSO = GameManager.instance.weaponSO;

        for (int i = 0; i < weaponSO.weapons.Length; i++)
        {
            if (weaponSO.weapons[i].name == itemName)
            {
                damageMultiple = weaponSO.weapons[i].damageMultiple;
            }
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

    protected abstract void OnUseSkill();
    protected abstract void OnAttack();

    public virtual void ProficiencyUp(float value)
    {
        proficiency += value;
        PlayerPrefs.SetFloat(itemName + "Proficiency", proficiency);
    }

    public virtual void DurabilityDown()
    {
        if (Random.Range(0, 5) < 1) //20%확률로 내구도가 0.1에서 0.9까지 줄어듬(내구도는 최대 100, 수리 가능)
        {
            float value = Random.Range(1, 10) / 10f;
            durability -= value;
            PlayerPrefs.SetFloat(itemName + "Durability", durability);
        }
    }

    public void Init(float durability, float proficiency)
    {
        weaponData.weaponName = itemName;
        weaponData.durability = durability;
        weaponData.proficiency = proficiency;
    }
}