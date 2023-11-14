using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public ItemSO itemSO = null;
    public EnemyBoxSO boxSO = null;
    public EnemySO enemySO = null;
    public ShopSO shopSO = null;
    public InventorySO inventorySO = null;
    public ShopGoodsSO shopGoodsSO = null;
    public WeaponSO weaponSO = null;
    public Transform mapTrm = null;
    public GameObject boxPf = null;
    public PoolSO pool = null;


    public List<AudioClip> audioClips = null;

    public bool isUIInput = false;

    [HideInInspector]
    public Transform player = null;
    [HideInInspector]
    public Transform canvas = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        canvas = GameObject.Find("Canvas").transform;
        player = GameObject.Find("Player").transform;

        CreateWeaponManager();
        CreateShopManager();
        CreateInventoryManager();
        CreateSoundManager();
        CreateSettingManager();
        CreateUIManager();
        CreateCameraManager();
        CreateMapManager();
        CreatePoolManager();
    }

    private void CreateShopManager()
    {
        if (ShopManager.instance != null) Destroy(ShopManager.instance);
        ShopManager.instance = gameObject.AddComponent<ShopManager>();
        ShopManager.instance.Init(itemSO, shopSO, FindAnyObjectByType<ItemBuyHelper>(), GameObject.Find("CannotBuyItem"));
        GameObject.Find("CannotBuyItem").SetActive(false);
    }

    private void CreateInventoryManager()
    {
        if (InventoryManager.instance != null) Destroy(InventoryManager.instance);
        InventoryManager.instance = gameObject.AddComponent<InventoryManager>();

        InventoryManager.instance.Init(inventorySO, itemSO, FindAnyObjectByType<ItemDevider>());
    }

    private void CreateSoundManager()
    {
        if (SoundManager.instance != null) Destroy(SoundManager.instance);
        SoundManager.instance = gameObject.AddComponent<SoundManager>();

        SoundManager.instance.Init(audioClips.ToArray());
    }

    private void CreateUIManager()
    {
        if (UIManager.instance != null) Destroy(UIManager.instance);
        UIManager.instance = gameObject.AddComponent<UIManager>();

        UIManager.instance.Init(canvas, player.GetComponent<PlayerStatus>(), itemSO, weaponSO);

    }

    private void CreateSettingManager()
    {
        if (SettingManager.instance != null) Destroy(SettingManager.instance);
        SettingManager.instance = gameObject.AddComponent<SettingManager>();
    }

    private void CreateWeaponManager()
    {
        if (WeaponManager.instance != null) Destroy(WeaponManager.instance);
        WeaponManager.instance = gameObject.AddComponent<WeaponManager>();
        WeaponManager.instance.Init(weaponSO);
    }

    private void CreateCameraManager()
    {
        if (CameraManager.instance != null) Destroy(CameraManager.instance);
        CameraManager.instance = GameObject.Find("Camera").AddComponent<CameraManager>();
        CameraManager.instance.Init();
    }

    private void CreatePoolManager()
    {
        if (PoolManager.instance != null) Destroy(PoolManager.instance);
        PoolManager.instance = gameObject.AddComponent<PoolManager>();
        PoolManager.instance.Init(pool, GameObject.Find("Pool").transform);
    }

    private void CreateMapManager()
    {
        if (PoolManager.instance != null) Destroy(PoolManager.instance);
        MapManager.Instance = gameObject.AddComponent<MapManager>();
        MapManager.Instance.Init(mapTrm);
    }

    public void Save()
    {
        if (player.GetComponent<OpenInventory>().MyInventory != null)
        {
            player.GetComponent<PlayerStatus>().JsonSave();
            player.GetComponent<OpenInventory>().MyInventory.JsonSave();
        }
    }

    public void Load()
    {
        if (player.GetComponent<OpenInventory>().MyInventory != null)
        {
            player.GetComponent<PlayerStatus>().JsonLoad();
            player.GetComponent<OpenInventory>().MyInventory.JsonLoad();
        }

    }
}
