using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;



public interface IDamageable
{
    void Damaged(float value, Vector2 hitPoint);
    void ReduceMaxHp(float value);
}

public class StatusSave
{
    public string[] weaponName = new string[3];
    public float[] durability = new float[3];
    public bool lantern;
    public float lanternDurability;
    public float playerMaxHp;
    public float playerAttact;
    public float playerSpeed;
    public int curProgress;     //임시이긴 한데 아마 현재 던전층수 나타낼 듯
}


public class PlayerStatus : MonoBehaviour, IDamageable
{
    private string path = "";

    private float playerMaxHp = 10f;        //최대HP
    private float reducedMaxHp = 0f;        //감소된 최대HP
    private float playerCurHp = 0f;         //현재 HP

    private float playerAttact = 0f;        //공격력
    private float reducedAttack = 0f;       //감소된 공격력

    private float playerSpeed = 5f;         //이동속도
    private float playerReducedSpeed = 0f;  //감소된 이동속도

    [SerializeField] private float skillChangeTime = 5f;    //스킬 교체 쿨타임
    private float skillChangeTimeDown = 0f;

    public float PlayerCurHp => playerCurHp;
    public float PlayerAttact => playerAttact;
    public float PlayerSpeed => playerSpeed;

    public Weapon[] mountingWeapon = new Weapon[2]; //장착 무기 2개
    private PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[2];   //무기 슬롯
    private Weapon curMountedWeapon = null; //현재 무기
    private List<Effect> effects = new List<Effect>();  //효과 ex)디버프, 버프
    private ItemSO itemSO = null;
    private Inventory myBackpack = null;
    private PlayerMove playerMove = null;

    private GameObject playerStatusUI = null;
    private OpenInventory playerInventory = null;
    private PlayerLanternSlot lantern;
    private PlayerInput playerBrain = null;
    private Image skillChangeCoolImg = null;

    private int curWeaponNum = 0;
    public int CurWeaponNum => curWeaponNum;
    public PlayerLanternSlot LanternSlot => lantern;


    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "PlayerStatus.json");

        playerMove = GetComponent<PlayerMove>();
        playerBrain = GetComponent<PlayerInput>();
        playerInventory = GetComponent<OpenInventory>();
        myBackpack = playerInventory.MyInventory;

        try
        {
            playerBrain.onOpenInventory += OnPressTab;
            playerBrain.onChangeWeapon += OnChangeWeapon;
        }
        catch
        {
            Debug.Log("인게임이 아녀");
        }

    }

    private void Update()
    {
        //디버그 용
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            Damaged(1, new Vector2(0, 0));
        }

        if (skillChangeTimeDown > 0)
        {
            skillChangeTimeDown -= Time.deltaTime;
            try
            {
            skillChangeCoolImg.fillAmount = skillChangeTimeDown / skillChangeTime;
            }
            catch
            {

            }
        }
    }

    public void OnChangeWeapon()
    {
        if (skillChangeTimeDown > 0) return;


        if (curWeaponNum == 0 && mountingWeapon[1] != null) curWeaponNum = 1;
        else if (curWeaponNum == 1 && mountingWeapon[0] != null) curWeaponNum = 0;
        else if (curMountedWeapon != null) return;

        skillChangeTimeDown = skillChangeTime;

        foreach (Weapon a in mountingWeapon)
        {
            if (a == null) continue;
            a.gameObject.SetActive(false);
        }

        if (mountingWeapon[curWeaponNum] != null)
        {
            curMountedWeapon = mountingWeapon[curWeaponNum];
            curMountedWeapon.gameObject.SetActive(true);
        }
        JsonSave();
    }

    private void OnPressTab()
    {
        JsonSave();
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

    public void SpeedDown(float value, float time)
    {
        playerReducedSpeed += value;
        StartCoroutine(SpeedUpRoutine(time));
    }

    IEnumerator SpeedUpRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        playerReducedSpeed = 0;
    }

    public void ChangeAttack(float value)
    {
        playerAttact = value;
    }

    public void Damaged(float value, Vector2 hitPoint)
    {
        playerMove.KnockBack((Vector2)transform.position - hitPoint);
        CameraManager.instance.ShakeCam(5, 1, 0.1f);

        playerCurHp -= value;

        playerCurHp = Mathf.Clamp(playerCurHp, 0, playerMaxHp - reducedMaxHp);
        UIManager.instance.SetHp(playerCurHp, playerMaxHp - reducedMaxHp);

        if(playerCurHp <= 100)
        {
            CameraManager.instance.ScreenHurt(true);
        }
        else 
        {
            CameraManager.instance.ScreenHurt(false);
        }

        if (playerCurHp <= 0)
        {
            Die();
        }
        Debug.Log(playerCurHp);
    }

    public void ReduceMaxHp(float value)
    {
        reducedMaxHp = value;
        playerCurHp -= value;

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

    public float GetPlayerSpeed()
    {
        return playerSpeed - playerReducedSpeed;
    }

    public PlayerWeaponSlot GetCurWeaponSlot()
    {
        return weaponSlots[curWeaponNum];
    }

    //이 새끼 문제 많으니까 나중에 무조건 바꿔야함 진짜 존나 문제 많음 걍 시발임ㅠㅠ 지금 도끼 줍는 용도로만 쓰이는데 먹으면 아이템이 사라지는 버그 생김 => 상점에서 아이템 사는거 참고 해서 고쳐야 함
    public void GetItem(Item item, float durability)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].assignedItem == null)
            {
                if (weaponSlots[i].SetItem(item, durability, true) == true)
                {
                    print(weaponSlots[i].name);
                    Destroy(item);
                    return;
                }
            }
        }
        if (myBackpack.SetItem(item))
        {
            //Destroy(item);
            return;
        }
        print("공간이 없다 게이야");
    }

    public void JsonSave()
    {
        StatusSave saves = new StatusSave();


        for (int i = 0; i < mountingWeapon.Length; i++)
        {
            if (mountingWeapon[i] != null)  //i번째 슬롯에 장착된 아이템이 있다면
            {
                saves.weaponName[i] = mountingWeapon[i].ItemName; //장착된 무기의 WeaponData를 저★장 해줘
                saves.durability[i] = mountingWeapon[i].Durability;
            }
            else
            {
                saves.weaponName[i] = "Null";
                saves.durability[i] = 100;
            }
        }

        if (lantern.GetItem() != null)
        {
            ItemWeapon mountingLantern = lantern.GetItem().GetComponent<ItemWeapon>();
            saves.lantern = true;
            saves.lanternDurability = mountingLantern.Durability;
        }
        else
        {
            saves.lantern = false;
            saves.lanternDurability = 100;
        }


        saves.playerMaxHp = playerMaxHp;
        saves.playerAttact = playerAttact;
        saves.playerSpeed = playerSpeed;

        path = Path.Combine(Application.dataPath, "PlayerStatus.json");

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
                    if (itemSO.items[j].itemName == saves.weaponName[i])
                    {
                        Item item = Instantiate(itemSO.items[j].pfItem.GetComponent<Item>());
                        item.Init(itemSO.items[j], 0);
                        item.GetComponent<ItemWeapon>().Init(saves.durability[i]);

                        weaponSlots[i].SetItem(item, saves.durability[i], true);
                    }
                }
            }

            if (saves.lantern == true)
            {
                for (int j = 0; j < itemSO.items.Count; j++)
                {
                    if (itemSO.items[j].itemName == "Lantern")
                    {
                        Item item = Instantiate(itemSO.items[j].pfItem.GetComponent<Item>());
                        item.Init(itemSO.items[j], 0);
                        item.GetComponent<ItemWeapon>().Init(saves.lanternDurability);

                        lantern.SetItem(item, saves.lanternDurability, true);
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

        this.playerStatusUI = playerStatusUI;
        playerStatusUI.SetActive(false);

        this.weaponSlots = weaponSlots;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].Init(weaponSO, itemSO, transform.Find("PlayerWeapon"), this);
        }

        try
        {
            skillChangeCoolImg = UIManager.instance.PlayerSkills.Find("Weapon/WeaponChangeCoolTime").GetComponent<Image>();
        }
        catch
        {
            Debug.Log("인게임이 아녀");
        }

        lantern.Init(itemSO);
        this.lantern = lantern;
        JsonLoad();
    }
}
