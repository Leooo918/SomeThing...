using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public ItemSO itemSO = null;
    public ShopSO shopSO = null;
    public InventorySO inventorySO = null;
    public ShopGoodsSO shopGoodsSO = null;
    public WeaponSO weaponSO = null;
    public Transform player = null;
    public Transform canvas = null;

    public List<AudioClip> audioClips = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        CreateShopManager();
        CreateInventoryManager();
        CreateSoundManager();

        canvas = GameObject.Find("Canvas").transform;
        player = GameObject.Find("Player").transform;
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
        if (UISoundManager.instance != null) Destroy(UISoundManager.instance);
        UISoundManager.instance = gameObject.AddComponent<UISoundManager>();

        UISoundManager.instance.Init(audioClips.ToArray());
    }
}
