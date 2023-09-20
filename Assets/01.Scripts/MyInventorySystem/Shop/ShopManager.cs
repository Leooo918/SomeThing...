using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShopSave
{
    public int shopLv;

    public List<string> itemNames = new List<string>();
    public List<int> itemAmounts = new List<int>();
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance = null;

    private ItemSO itemSO = null;
    private ShopSO shopSO = null;
    private Shop shop = null;
    private ItemBuyHelper itemBuyHelper = null;
    private GameObject errorScreen = null;
    private int shopLv = 0;

    public bool isBuying = false;

    public ItemBuyHelper ItemBuyHelper => itemBuyHelper;

    private string path = "";

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "Shop" + "database.json");
    }

    private void Start()
    {
        itemSO = InventoryManager.instance.itemSO;
    }

    public Item[] BuyItem(ShopGoods itemToBuy, int amount)  //살 아이템을 살 개수만큼 소환하고 아이템 배열을 반환
    {
        List<Item> items = new List<Item>();

        for (int i = 0; i < itemSO.items.Count; i++)
        {
            if (itemSO.items[i].itemName == itemToBuy.goodsName)
            {
                for (int j = 0; j < amount; j++)
                {
                    Item item = Instantiate(itemSO.items[i].pfItem).GetComponent<Item>();

                    RectTransform rect = item.GetComponent<RectTransform>();    //부모 지정해주고 크기 1,1,1로 바꾸고 
                    rect.SetParent(GameObject.Find("Canvas").transform);
                    rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    item.transform.localScale = new Vector3(1, 1, 1);

                    item.Init(itemSO.items[i], 0);
                    items.Add(item);
                }

                return items.ToArray();
            }
        }
        for (int i = 0; i < itemSO.expendableItems.Count; i++)
        {
            if (itemSO.expendableItems[i].itemName == itemToBuy.goodsName)
            {
                while (amount > 0)
                {
                    ExpendableItem expendableItem = Instantiate(itemSO.expendableItems[i].pfItem).GetComponent<ExpendableItem>();

                    RectTransform rect = expendableItem.GetComponent<RectTransform>();                    //부모 지정해주고 크기 1,1,1로 바꾸고 
                    rect.parent = transform.root;
                    expendableItem.transform.localScale = new Vector3(1, 1, 1);


                    if (amount > itemSO.expendableItems[i].maxItemNum)
                    {
                        amount -= itemSO.expendableItems[i].maxItemNum;
                        expendableItem.Init(itemSO.expendableItems[i], itemSO.expendableItems[i].maxItemNum, 0);
                    }
                    else
                    {
                        expendableItem.Init(itemSO.expendableItems[i], amount, 0);
                        amount = 0;
                    }
                    items.Add(expendableItem as Item);
                }

                return items.ToArray();
            }
        }
        return null;
    }

    public int SellItem(Item sellItem, int amount) //팔 아이템과 개수를 받아 받을 돈을 반환
    {
        for (int i = 0; i < itemSO.items.Count; i++)
        {
            if (itemSO.items[i].itemName == sellItem.itemName)
            {
                return itemSO.items[i].itemValue * amount;
            }
        }
        for (int i = 0; i < itemSO.expendableItems.Count; i++)
        {
            if (itemSO.expendableItems[i].itemName == sellItem.itemName)
            {
                return itemSO.expendableItems[i].itemValue * amount;
            }
        }
        return 0;
    }

    public void SaveShop()
    {
        ShopSave save = new ShopSave();

        save.shopLv = shopLv;

        for (int i = 0; i < shop.shopGoodses.Count; i++)
        {
            save.itemNames.Add(shop.shopGoodses[i].goodsName);
            save.itemAmounts.Add(shop.shopGoodses[i].itemAmount);
            shop.shopGoodses[i].SetText();
        }

        string json = JsonUtility.ToJson(save, true);                                                                  //걍 다 json파일로 바꿔 저장해
        File.WriteAllText(path, json);
    }

    public void LoadShop()
    {
        ShopSave saves = new ShopSave();

        if (!File.Exists(path))                                                                         //불러오는데 json파일이 없다면 
        {
            SaveShop();                                                                                 //저장하고
        }
        else                                                                                            //json파일이 있다면
        {
            string loadJson = File.ReadAllText(path);                                                   //파일 Text를 읽고
            saves = JsonUtility.FromJson<ShopSave>(loadJson);
            

            for (int i = 0; i < saves.itemNames.Count; i++)
            {
                for (int j = 0; j < shop.shopGoodses.Count; j++)
                {
                    if (shop.shopGoodses[j].goodsName == saves.itemNames[i])
                    {
                        shop.shopGoodses[j].itemAmount = saves.itemAmounts[i];
                        shop.shopGoodses[j].SetText();
                        if (shop.shopGoodses[j].itemAmount <= 0) shop.shopGoodses[i].SoldOut();
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            print("야호");
            ResetShop();
        }
    }

    public void ResetShop()
    {
        File.Delete(path);
        Destroy(shop.gameObject);
        Init(itemSO, shopSO, itemBuyHelper, errorScreen, true);
    }

    public void Init(ItemSO itemSO, ShopSO shopSO, ItemBuyHelper itemBuyHelper, GameObject errorScreen, bool reset = false)
    {
        this.itemSO = itemSO;
        this.itemBuyHelper = itemBuyHelper;
        this.shopSO = shopSO;
        this.errorScreen = errorScreen;
        itemBuyHelper.gameObject.SetActive(false);

        isBuying = false;

        if (PlayerPrefs.HasKey("ShopLv") == false)
        {
            PlayerPrefs.SetInt("ShopLv", 0);
        }
        shopLv = PlayerPrefs.GetInt("ShopLv");

        for (int i = 0; i < shopSO.shops.Length; i++)
        {
            if (shopSO.shops[i].shopLv == shopLv)
            {
                shop = Instantiate(shopSO.shops[i].pfShop).GetComponent<Shop>();

                shop.transform.SetParent(GameObject.Find("Canvas").transform);
                shop.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                shop.transform.localScale = new Vector3(1, 1, 1);
                shop.transform.SetSiblingIndex(1);

                shop.Init(shopSO.shopGoodsSO, itemBuyHelper, errorScreen);
            }
        }

        //shop.Init();
    }
}
