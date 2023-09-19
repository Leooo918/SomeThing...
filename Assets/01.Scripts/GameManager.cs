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

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        CreateShopManager();
        CreateInventoryManager();
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
        if (ShopManager.instance != null) Destroy(ShopManager.instance);
        ShopManager.instance = gameObject.AddComponent<ShopManager>();

        
    }
}
