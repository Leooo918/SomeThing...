using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class ItemSave
{
    public string inventoryName;

    public List<string> itemName = new List<string>();
    public List<int> itemAmount = new List<int>();
    public List<int> positionX = new List<int>();
    public List<int> positionY = new List<int>();
    public List<float> rotation = new List<float>();
}




public class Inventory : MonoBehaviour
{
    public string inventoryName = "";       //이 인벤토리의 이름 지정해주고 중복 있지 않게 OK?
    private InventorySO inventorySO = null;
    private ItemSO itemSO = null;
    private InventoryStruct inventoryStruct;

    private string path = "";

    private int width = 0;
    private int height = 0;

    public Slot[,] slots;

    public int Width => width;
    public int Height => height;


    private void Start()
    {
        path = Path.Combine(Application.dataPath, "InventorySaveDatabases\\" + inventoryName + "database.json");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            JsonSave();
        }
    }

    public void SortingInventory()
    {
        UISoundManager.instance.PlayOneShot(Sound.ButtonClilckSound);

        List<Item> itemList = new List<Item>();
        List<ExpendableItem> expendableItemList = new List<ExpendableItem>();

        for (int j = 0; j < slots.GetLength(1); j++)
        {
            for (int k = 0; k < slots.GetLength(0); k++)
            {
                if (slots[k, j].IsAssignedItemsOriginPos == true)      //저장해야될 모든 인벤토리의 슬롯을 돌면서 아이템의 왼쪽아래쪽이 할당된 슬롯을 찾아
                {
                    Item item = slots[k, j].assignedItem;

                    if (item == null)
                    {
                        return;
                    }

                    if (item.TryGetComponent<ExpendableItem>(out ExpendableItem expendableItem))
                    {
                        expendableItemList.Add(expendableItem);
                    }
                    else
                    {
                        itemList.Add(item);
                    }

                    for (int l = 0; l < item.assignedSlot.Count; l++)
                    {
                        item.assignedSlot[l].assignedItem = null;
                    }
                    item.assignedSlot.Clear();
                }
            }
        }


        bool isItemSet = false;

        for (int k = 0; k < itemList.Count; k++)
        {
            isItemSet = false;
            for (int i = slots.GetLength(1) - 1; i >= 0; i--)
            {
                for (int j = 0; j < slots.GetLength(0); j++)
                {
                    if (slots[j, i].CheckCanSetPosition(itemList[k]) == true)
                    {
                        slots[j, i].SetItem(itemList[k]);
                        //slots[j, i].SetIsOriginPos();
                        isItemSet = true;
                        break;
                    }
                }
                if (isItemSet == true)
                {
                    break;
                }
            }
        }

        for (int k = 0; k < expendableItemList.Count; k++)
        {
            isItemSet = false;
            for (int i = slots.GetLength(1) - 1; i >= 0; i--)
            {
                for (int j = 0; j < slots.GetLength(0); j++)
                {
                    if (slots[j, i].CheckCanSetPosition(expendableItemList[k]) == true)
                    {
                        print(j + " " + i);
                        slots[j, i].SetItem(expendableItemList[k]);
                        //slots[j, i].SetIsOriginPos();
                        isItemSet = true;
                        break;
                    }
                }
                if (isItemSet == true)
                {
                    break;
                }
            }
        }
    }

    public void JsonSave()
    {
        ItemSave saves = new ItemSave();                                                                                //저장할 클래스

        saves.inventoryName = inventoryName;                                                                            //이름 지정 해주고

        for (int j = 0; j < slots.GetLength(1); j++)
        {
            for (int k = 0; k < slots.GetLength(0); k++)
            {
                if (slots[k, j].IsAssignedItemsOriginPos == true)      //저장해야될 모든 인벤토리의 슬롯을 돌면서 아이템의 왼쪽아래쪽이 할당된 슬롯을 찾아
                {
                    Item item = slots[k, j].assignedItem;

                    saves.itemName.Add(item.ItemName);                                                              //저장해(아이템 이름, 위치, 개수)
                    saves.positionX.Add(k);
                    saves.positionY.Add(j);
                    saves.rotation.Add(item.GetComponent<RectTransform>().eulerAngles.z);

                    if (item.GetComponent<ExpendableItem>() != null)
                    {
                        ExpendableItem expendableItem = item.GetComponent<ExpendableItem>();
                        saves.itemAmount.Add(expendableItem.currentItemNum);
                    }
                    else
                        saves.itemAmount.Add(1);
                }
            }

        }

        string json = JsonUtility.ToJson(saves, true);                                                                  //걍 다 json파일로 바꿔 저장해
        File.WriteAllText(path, json);
    }

    public void JsonLoad()
    {
        ItemSave saves = new ItemSave();

        if (!File.Exists(path))                                                                         //불러오는데 json파일이 없다면 
        {
            JsonSave();                                                                                 //저장하고
        }
        else                                                                                            //json파일이 있다면
        {
            string loadJson = File.ReadAllText(path);                                                   //파일 Text를 읽고
            saves = JsonUtility.FromJson<ItemSave>(loadJson);

            for (int i = 0; i < saves.itemName.Count; i++)
            {
                for (int j = 0; j < itemSO.items.Count; j++)
                {
                    if (itemSO.items[j].itemName == saves.itemName[i])                                  //아이템SO에서 저장된 이름의 아이템과 같은 이름의 아이템을 찾아
                    {
                        Vector2Int v = new Vector2Int(saves.positionX[i], saves.positionY[i]);          //아이템의 위치를 지정하고
                        SetItemWithSPosition(v, j, saves.rotation[i]);                                              //아이템을 지정한다(그냥 아이템은 위치 지정)
                    }
                }
                for (int j = 0; j < itemSO.expendableItems.Count; j++)
                {
                    if (itemSO.expendableItems[j].itemName == saves.itemName[i])
                    {
                        Vector2Int v = new Vector2Int(saves.positionX[i], saves.positionY[i]);          //아이템의 위치를 지정하고
                        SetItemWithSPosition(v, j, saves.rotation[i], true, saves.itemAmount[i]);                   //아이템을 지정한다(나눌 수 있는 아이템은 위치 지정, 개수 지정)
                    }
                }
            }
        }
    }

    private void SetItemWithSPosition(Vector2Int originPos, int arrayNum, float rotation, bool isExpendableItem = false, int itemAmount = 0)
    {
        Item item = null;
        ExpendableItem expendableItem = null;

        if (isExpendableItem == false)
        {
            item = Instantiate(itemSO.items[arrayNum].pfItem).GetComponent<Item>();     //인스턴스로 만들어 주고
        }
        else
        {
            expendableItem = Instantiate(itemSO.expendableItems[arrayNum].pfItem).GetComponent<ExpendableItem>();
            item = expendableItem.GetComponent<Item>();
        }

        RectTransform rect = item.GetComponent<RectTransform>();                    //부모 지정해주고 크기 1,1,1로 바꾸고 
        rect.transform.SetParent(transform.root);
        item.transform.localScale = new Vector3(1, 1, 1);



        if (isExpendableItem == false)                                                  //나눌 수 없는 아이템이면
        {
            Normal normalItem = new Normal();
            normalItem = itemSO.items[arrayNum];                                        //일반 아이템의 구조체 들고와

            item.Init(normalItem, rotation);                                                      //아이템 Init해주고
        }
        else
        {
            Expendable expendable = new Expendable();
            expendable = itemSO.expendableItems[arrayNum];

            expendableItem.Init(expendable, itemAmount, rotation);
        }

        slots[originPos.x, originPos.y].SetItem(item);                             //슬롯에서 아이템을 셋팅 해줘
        //slots[originPos.x, originPos.y].SetIsOriginPos();
        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y, 0);//z값 0으로

        for (int i = 0; i < item.assignedSlot.Count; i++)
        {
            item.lastSlot.Add(item.assignedSlot[i]);
        }
    }

    public bool SetItem(Item item, int amount = 1)
    {
        print(amount);
        for (int i = slots.GetLength(1) - 1; i >= 0; i--)
        {
            for (int j = 0; j < slots.GetLength(0); j++)
            {
                if (slots[j, i].CheckCanSetPosition(item) == true)  //그 슬롯으로부터 지정된 아이템의 사이즈만큼 
                {
                    print(j + " " + i);
                    if (item.TryGetComponent<ExpendableItem>(out ExpendableItem expendableItem))
                    {
                        slots[j, i].SetItem(expendableItem);
                    }
                    else
                    {
                        slots[j, i].SetItem(item);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public void Init(ItemSO itemSO, InventorySO inventorySO, InventoryStruct inventoryStruct)
    {
        path = Path.Combine(Application.dataPath, "InventorySaveDatabases\\" + inventoryName + "database.json");

        this.itemSO = itemSO;
        this.inventorySO = inventorySO;

        this.inventoryStruct = inventoryStruct;
        inventoryName = inventoryStruct.inventoryName;

        width = inventoryStruct.width;                                      //인벤토리의 가로
        height = inventoryStruct.height;                                    //인벤토리의 세로

        slots = new Slot[width, height];                                    //슬롯의 크기 지정

        Transform parent = transform.Find("InventoryBackground");           //인벤토리 Background의 자식으로 둘거다!

        RectTransform background = parent.GetComponent<RectTransform>();
        background.sizeDelta = new Vector2(width * 100, height * 100);

        GameObject prefab = inventoryStruct.pfSlot;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                slots[j, i] = Instantiate(prefab, parent).GetComponent<Slot>();
                slots[j, i].Init(j, i, this);
            }
        }


        JsonLoad();                                                         //슬롯까지 다 깔고 로드 한번 해줌
        OpenInventory[] openInventory = FindObjectsByType<OpenInventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (OpenInventory inventory in openInventory)
        {
            if (inventory.inventoryName == inventoryName)
            {
                inventory.myInventory = this;
                inventory.InventoryClose();
                return;
            }
        }
    }
}