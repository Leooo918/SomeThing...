using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;

public interface IDamageable
{
    void Damaged(float value);
    void ReduceMaxHp(float value);
}

public class StatusSave
{
    public string[] itemName = new string[3];
    public float playerMaxHp;
    public float playerAttact;
    public float playerSpeed;
    public int curProgress;     //임시
}


public class PlayerStatus : MonoBehaviour, IDamageable
{
    private string path = "";

    private float playerMaxHp = 10f;        //최대HP
    private float reducedMaxHp = 0f;        //감소된 최대HP
    private float playerCurHp = 0f;         //현재 HP
    
    private float playerAttact = 0f;        //공격력
    private float reducedAttack = 0f;       //감소된 공격력

    private float playerSpeed = 0f;         //이동속도
    private float playerReducedSpeed = 0f;  //감소된 이동속도

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
        path = Path.Combine(Application.dataPath, "WeaponSave" + "database.json");

        playerBrain = GetComponent<PlayerInput>();
        playerStatusUI = GameObject.Find("PlayerStatusbackground");
        playerInventory = GetComponent<OpenInventory>();
        weaponSelector = transform.Find("WeaponSelect").GetComponent<WeaponSelector>();

        myBackpack = playerInventory.myInventory;

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
        JsonLoad();
        playerStatusUI.SetActive(false);
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
    }

    public void ReduceMaxHp(float value)
    {
        reducedMaxHp = value;
    }

    public void JsonSave()
    {
        StatusSave saves = new StatusSave();

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

            playerMaxHp = saves.playerMaxHp;
            playerAttact = saves.playerAttact;
            playerSpeed = saves.playerSpeed;
        }
    }
}
