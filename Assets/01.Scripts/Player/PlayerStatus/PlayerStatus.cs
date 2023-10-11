using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;

public interface IDamageable
{
    void Damaged(float value);
    void ReduceMaxHp(float value);
}

public class WeaponSave
{
    public string[] itemName = new string[3];
}


public class PlayerStatus : MonoBehaviour, IDamageable
{
    private string path = "";

    private float playerMaxHp = 10f;
    private float reducedMaxHp = 0f;

    private float playerCurHp = 0f;
    private float playerAttact = 0f;
    private float playerSpeed = 0f;

    public float PlayerCurHp => playerCurHp;
    public float PlayerAttact => playerAttact;
    public float PlayerSpeed => playerSpeed;

    public Weapon[] mountingWeapon = new Weapon[3];
    private PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[3];
    private Weapon curMountedWeapon = null;
    private List<Effect> effects = new List<Effect>();
    private ItemSO itemSO = null;
    private WeaponSO weaponSO = null;
    private OpenInventory inventory = null;
    private Inventory myBackpack = null;

    private GameObject playerStatusUI = null;
    private WeaponSelector weaponSelector = null;
    private OpenInventory playerInventory = null;
    private PlayerInput playerBrain = null;

    private int curWeaponNum = 0;

    public int CurWeaponNum => curWeaponNum;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "WeaponSave" + "database.json");

        playerBrain = GetComponent<PlayerInput>();
        playerStatusUI = GameObject.Find("PlayerStatusbackground");
        playerInventory = GameObject.Find("MyBackpack").GetComponent<OpenInventory>();
        weaponSelector = transform.Find("WeaponSelect").GetComponent<WeaponSelector>();

        inventory = GameObject.Find("MyBackpack").GetComponent<OpenInventory>();
        myBackpack = inventory.myInventory;

        weaponSlots = FindObjectsByType<PlayerWeaponSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        playerBrain.onOpenInventory += OnPressTab;
    }

    private void Start()
    {
        itemSO = GameManager.instance.itemSO;
        weaponSO = GameManager.instance.weaponSO;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].Init(weaponSO, itemSO, transform.Find("PlayerWeapon"), this);
        }
        LoadWeapon();
        playerStatusUI.SetActive(false);
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
        playerInventory.myInventory.transform.SetParent(playerStatusUI.transform);
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
    }

    public void ReduceMaxHp(float value)
    {
        reducedMaxHp = value;
    }

    public void SaveWeapon()
    {
        WeaponSave saves = new WeaponSave();

        for (int i = 0; i < 3; i++)
        {
            if (mountingWeapon[i] != null)
            {
                saves.itemName[i] = mountingWeapon[i].ItemName;
            }

            for (int j = 0; j < weaponSO.weapons.Length; j++)
            {
                if (mountingWeapon[i] != null)
                {
                    if (weaponSO.weapons[j].name == mountingWeapon[i].ItemName)
                    {
                        GameObject g = Instantiate(weaponSO.weapons[j].weaponImageObj, weaponSelector.transform.GetChild(1 + j));
                        g.transform.position = new Vector3(0, 0, 0);
                    }
                }
            }
        }

        string json = JsonUtility.ToJson(saves, true);
        File.WriteAllText(path, json);
    }

    public void LoadWeapon()
    {
        WeaponSave saves = new WeaponSave();

        if (!File.Exists(path))
        {
            SaveWeapon();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saves = JsonUtility.FromJson<WeaponSave>(loadJson);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < itemSO.items.Count; j++)
                {
                    if (itemSO.items[j].itemName == saves.itemName[i])
                    {
                        Item item = Instantiate(itemSO.items[j].pfItem.GetComponent<Item>());
                        item.Init(itemSO.items[j], 0);
                        weaponSlots[i].SetWeapon(item, true);
                        Destroy(item.gameObject);
                    }
                }
            }
        }
    }
}
