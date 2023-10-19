using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;

public struct WeaponSave
{
    public string weaponName;
    public float durability;
    public float proficiency;
}


public interface IDamageable
{
    void Damaged(float value);
    void ReduceMaxHp(float value);
}

public class StatusSave
{
    public WeaponSave[] weapon = new WeaponSave[3];
    public float playerMaxHp;
    public float playerAttact;
    public float playerSpeed;
    public int curProgress;     //�ӽ��̱� �ѵ� �Ƹ� ���� �������� ��Ÿ�� ��
}


public class PlayerStatus : MonoBehaviour, IDamageable
{
    private string path = "";

    private float playerMaxHp = 10f;        //�ִ�HP
    private float reducedMaxHp = 0f;        //���ҵ� �ִ�HP
    private float playerCurHp = 0f;         //���� HP

    private float playerAttact = 0f;        //���ݷ�
    private float reducedAttack = 0f;       //���ҵ� ���ݷ�

    private float playerSpeed = 0f;         //�̵��ӵ�
    private float playerReducedSpeed = 0f;  //���ҵ� �̵��ӵ�

    public float PlayerCurHp => playerCurHp;
    public float PlayerAttact => playerAttact;
    public float PlayerSpeed => playerSpeed;

    public Weapon[] mountingWeapon = new Weapon[3];
    private PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[3];
    private Weapon curMountedWeapon = null;
    private List<Effect> effects = new List<Effect>();
    private ItemSO itemSO = null;
    private WeaponSO weaponSO = null;
    private Inventory myBackpack = null;

    private GameObject playerStatusUI = null;
    private WeaponSelector weaponSelector = null;
    private OpenInventory playerInventory = null;
    private PlayerInput playerBrain = null;

    private int curWeaponNum = 0;

    public int CurWeaponNum => curWeaponNum;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "PlayerStatus.json");

        weaponSelector = transform.Find("WeaponSelect").GetComponent<WeaponSelector>();
        playerBrain = GetComponent<PlayerInput>();
        playerInventory = GetComponent<OpenInventory>();
        myBackpack = playerInventory.MyInventory;

        playerBrain.onOpenInventory += OnPressTab;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            JsonSave();
        }
    }

    public void OnChangeWeapon(int num)
    {
        foreach (Weapon a in mountingWeapon)
        {
            if (a == null) continue;
            a.gameObject.SetActive(false);
            print(a.ItemName);
        }

        curWeaponNum = num;
        if (mountingWeapon[curWeaponNum] != null)
        {
            curMountedWeapon = mountingWeapon[curWeaponNum];
            curMountedWeapon.gameObject.SetActive(true);
        }
    }

    private void OnPressTab()
    {
        if (playerStatusUI.activeSelf == true)
        {
            ClosePlayerStatus();
        }
        else
        {
            OpenPlayerStatus();
        }
    }

    private void OpenPlayerStatus()
    {
        playerStatusUI.SetActive(true);
        playerStatusUI.transform.SetAsFirstSibling();
        playerInventory.InventoryOpen();
    }

    private void ClosePlayerStatus()
    {
        playerStatusUI.SetActive(false);
        playerInventory.InventoryClose();
    }

    public void ChangeSpeed(float value)
    {
        playerSpeed = value;
    }

    public void ChangeAttack(float value)
    {
        playerAttact = value;
    }

    public void Damaged(float value)
    {
        playerCurHp -= value;

        playerCurHp = Mathf.Clamp(playerCurHp, 0, playerMaxHp - reducedMaxHp);
        UIManager.instance.SetHp(playerCurHp, playerMaxHp - reducedMaxHp);

        if (playerCurHp <= 0)
        {
            Die();
        }
    }

    public void ReduceMaxHp(float value)
    {
        reducedMaxHp = value;

        playerCurHp = Mathf.Clamp(playerCurHp, 0, playerMaxHp - reducedMaxHp);
        UIManager.instance.SetHp(playerCurHp, playerMaxHp - reducedMaxHp);


        if (playerCurHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

    }

    public void JsonSave()
    {
        StatusSave saves = new StatusSave();

        for (int i = 0; i < 3; i++)
        {
            if (mountingWeapon[i] != null)  //i��° ���Կ� ������ �������� �ִٸ�
            {
                print(mountingWeapon[i].WeaponData.weaponName);
                saves.weapon[i] = mountingWeapon[i].WeaponData; //������ ������ WeaponData�� ������ ����
            }

            //for (int j = 0; j < weaponSO.weapons.Length; j++)
            //{
            //    if (mountingWeapon[i] != null)
            //    {
            //        if (weaponSO.weapons[j].name == mountingWeapon[i].ItemName)
            //        {
            //            GameObject g = Instantiate(weaponSO.weapons[j].weaponImageObj, weaponSelector.transform.GetChild(1 + j));
            //            g.transform.position = new Vector3(0, 0, 0);
            //        }
            //    }
            //}
        }

        saves.playerMaxHp = playerMaxHp;
        saves.playerAttact = playerAttact;
        saves.playerSpeed = playerSpeed;

        string json = JsonUtility.ToJson(saves, true);
        File.WriteAllText(path, json);
    }

    public void JsonLoad()
    {
        StatusSave saves = new StatusSave();

        if (!File.Exists(path))
        {
            JsonSave();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saves = JsonUtility.FromJson<StatusSave>(loadJson);

            for (int i = 0; i < 3; i++)
            {
                WeaponSave weapon = saves.weapon[i];

                for (int j = 0; j < itemSO.items.Count; j++)
                {
                    if (itemSO.items[j].itemName == weapon.weaponName)
                    {
                        Item item = Instantiate(itemSO.items[j].pfItem.GetComponent<Item>());
                        item.Init(itemSO.items[j], 0);

                        weaponSlots[i].SetItem(item, weapon.durability, weapon.proficiency, true);
                    }
                }
            }

            playerMaxHp = saves.playerMaxHp;
            playerAttact = saves.playerAttact;
            playerSpeed = saves.playerSpeed;
        }
    }

    public void Init(ItemSO itemSO, WeaponSO weaponSO, GameObject playerStatusUI, PlayerWeaponSlot[] weaponSlots, PlayerLanternSlot lantern)
    {
        this.itemSO = itemSO;
        this.weaponSO = weaponSO;

        this.playerStatusUI = playerStatusUI;
        playerStatusUI.SetActive(false);

        this.weaponSlots = weaponSlots;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].Init(weaponSO, itemSO, transform.Find("PlayerWeapon"), this);
        }
        lantern.Init(itemSO);

        JsonLoad();
    }
}
