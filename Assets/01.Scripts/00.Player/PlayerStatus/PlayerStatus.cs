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

    private float playerSpeed = 5f;         //�̵��ӵ�
    private float playerReducedSpeed = 0f;  //���ҵ� �̵��ӵ�

    [SerializeField] private float skillChangeTime = 5f;    //��ų ��ü ��Ÿ��
    private float skillChangeTimeDown = 0f;

    public float PlayerCurHp => playerCurHp;
    public float PlayerAttact => playerAttact;
    public float PlayerSpeed => playerSpeed;

    public Weapon[] mountingWeapon = new Weapon[2]; //���� ���� 2��
    private PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[2];   //���� ����
    private Weapon curMountedWeapon = null; //���� ����
    private List<Effect> effects = new List<Effect>();  //ȿ�� ex)�����, ����
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
            Debug.Log("�ΰ����� �Ƴ�");
        }

    }

    private void Update()
    {
        //����� ��
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

    //�� ���� ���� �����ϱ� ���߿� ������ �ٲ���� ��¥ ���� ���� ���� �� �ù��ӤФ� ���� ���� �ݴ� �뵵�θ� ���̴µ� ������ �������� ������� ���� ���� => �������� ������ ��°� ���� �ؼ� ���ľ� ��
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
        print("������ ���� ���̾�");
    }

    public void JsonSave()
    {
        StatusSave saves = new StatusSave();


        for (int i = 0; i < mountingWeapon.Length; i++)
        {
            if (mountingWeapon[i] != null)  //i��° ���Կ� ������ �������� �ִٸ�
            {
                saves.weaponName[i] = mountingWeapon[i].ItemName; //������ ������ WeaponData�� ������ ����
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
            Debug.Log("�ΰ����� �Ƴ�");
        }

        lantern.Init(itemSO);
        this.lantern = lantern;
        JsonLoad();
    }
}
