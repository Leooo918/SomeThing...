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
    public List<float> durability = new List<float>();
}




public class Inventory : MonoBehaviour
{
    public string inventoryName = "";       //�� �κ��丮�� �̸� �������ְ� �ߺ� ���� �ʰ� OK?
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

        for (int j = 0; j < slots.GetLength(0); j++)
        {
            for (int k = 0; k < slots.GetLength(1); k++)
            {
                if (slots[j, k].IsAssignedItemsOriginPos == true)      //�����ؾߵ� ��� �κ��丮�� ������ ���鼭 �������� ���ʾƷ����� �Ҵ�� ������ ã��
                {
                    Item item = slots[j, k].assignedItem;

                    if (item == null)
                    {
                        continue;
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


        for (int i = 0; i < itemList.Count; i++)
        {
            for (int k = slots.GetLength(1) - 1; k >= 0; k--)
            {
                for (int j = 0; j < slots.GetLength(0); j++)
                {
                    if (slots[j, k].CheckCanSetPosition(itemList[i]) == true)
                    {
                        slots[j, k].SetItem(itemList[i]);
                        goto a;
                    }
                }
            }
        a:;
        }

        for (int i = 0; i < expendableItemList.Count; i++)
        {
            for (int k = slots.GetLength(1) - 1; k >= 0; k--)
            {
                for (int j = 0; j < slots.GetLength(0); j++)
                {
                    if (slots[j, k].CheckCanSetPosition(expendableItemList[i]) == true)
                    {
                        slots[j, k].SetItem(expendableItemList[i]);
                        goto b;
                    }
                }
            }
        b:;
        }
    }

    public void JsonSave()
    {
        ItemSave saves = new ItemSave();                                //������ Ŭ����

        saves.inventoryName = inventoryName;                            //�̸� ���� ���ְ�

        for (int j = 0; j < slots.GetLength(1); j++)
        {
            for (int k = 0; k < slots.GetLength(0); k++)
            {
                if (slots[k, j].IsAssignedItemsOriginPos == true)       //�����ؾߵ� ��� �κ��丮�� ������ ���鼭 �������� ���ʾƷ����� �Ҵ�� ������ ã��
                {
                    Item item = slots[k, j].assignedItem;

                    if (item == null) continue;

                    saves.itemName.Add(item.ItemName);                  //������(������ �̸�, ��ġ, ����)
                    saves.positionX.Add(k);
                    saves.positionY.Add(j);


                    if (item.enabled == true)
                    {
                        saves.rotation.Add(item.GetComponent<RectTransform>().eulerAngles.z);
                    }
                    else
                    {
                        saves.rotation.Add(0);
                    }

                    if(item.TryGetComponent<ItemWeapon>(out ItemWeapon w))
                    {
                        saves.durability.Add(w.Durability);
                    }
                    else
                    {
                        saves.durability.Add(100);
                    }

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

        string json = JsonUtility.ToJson(saves, true);                                                                  //�� �� json���Ϸ� �ٲ� ������
        File.WriteAllText(path, json);
    }

    public void JsonLoad()
    {
        ItemSave saves = new ItemSave();

        if (!File.Exists(path))                                                                         //�ҷ����µ� json������ ���ٸ� 
        {
            JsonSave();                                                                                 //�����ϰ�
        }
        else                                                                                            //json������ �ִٸ�
        {
            string loadJson = File.ReadAllText(path);                                                   //���� Text�� �а�
            saves = JsonUtility.FromJson<ItemSave>(loadJson);

            for (int i = 0; i < saves.itemName.Count; i++)
            {
                for (int j = 0; j < itemSO.items.Count; j++)
                {
                    if (itemSO.items[j].itemName == saves.itemName[i])                                  //������SO���� ����� �̸��� �����۰� ���� �̸��� �������� ã��
                    {
                        Vector2Int v = new Vector2Int(saves.positionX[i], saves.positionY[i]);          //�������� ��ġ�� �����ϰ�
                        SetItemWithPosition(v, j, saves.rotation[i], saves.durability[i]);              //�������� �����Ѵ�(�׳� �������� ��ġ ����)
                    }
                }
                for (int j = 0; j < itemSO.expendableItems.Count; j++)
                {
                    if (itemSO.expendableItems[j].itemName == saves.itemName[i])
                    {
                        Vector2Int v = new Vector2Int(saves.positionX[i], saves.positionY[i]);          //�������� ��ġ�� �����ϰ�
                        SetItemWithPosition(v, j, saves.rotation[i], saves.durability[i], true, saves.itemAmount[i]);      //�������� �����Ѵ�(���� �� �ִ� �������� ��ġ ����, ���� ����)
                    }
                }
            }
        }
    }

    public void ResetInventory()
    {
        ItemSave saves = new ItemSave();

        string json = JsonUtility.ToJson(saves, true);                                                                  //�� �� json���Ϸ� �ٲ� ������
        File.WriteAllText(path, json);
    }

    private void SetItemWithPosition(Vector2Int originPos, int arrayNum, float rotation,float durability, bool isExpendableItem = false, int itemAmount = 0)
    {
        Item item = null;
        ExpendableItem expendableItem = null;

        if (isExpendableItem == false)
        {
            item = Instantiate(itemSO.items[arrayNum].pfItem).GetComponent<Item>();     //�ν��Ͻ��� ����� �ְ�
        }
        else
        {
            expendableItem = Instantiate(itemSO.expendableItems[arrayNum].pfItem).GetComponent<ExpendableItem>();
            item = expendableItem.GetComponent<Item>();
        }

        RectTransform rect = item.GetComponent<RectTransform>();                            //�θ� �������ְ� ũ�� 1,1,1�� �ٲٰ�
        rect.transform.SetParent(transform.root);
        item.transform.localScale = new Vector3(1, 1, 1);



        if (isExpendableItem == false)                                                      //���� �� ���� �������̸�
        {
            Normal normalItem = itemSO.items[arrayNum];                                     //�Ϲ� �������� ����ü ����

            item.Init(normalItem, rotation);    //������ Init���ְ�
            if(item.TryGetComponent<ItemWeapon>(out ItemWeapon w))
            {
                w.Init(durability);
            }
        }
        else
        {
            Expendable expendable = itemSO.expendableItems[arrayNum];

            expendableItem.Init(expendable, itemAmount, rotation);
        }

        slots[originPos.x, originPos.y].SetItem(item);                                      //���Կ��� �������� ���� ����

        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y, 0); //z�� 0����

        for (int i = 0; i < item.assignedSlot.Count; i++)
        {
            item.lastSlot.Add(item.assignedSlot[i]);
        }
    }

    public bool SetItem(Item item, int amount = 1)
    {
        for (int i = slots.GetLength(1) - 1; i >= 0; i--)
        {
            for (int j = 0; j < slots.GetLength(0); j++)
            {
                if (slots[j, i].CheckCanSetPosition(item) == true)  //�� �������κ��� ������ �������� �����ŭ
                {
                    if (item.TryGetComponent<ExpendableItem>(out ExpendableItem expendableItem))
                    {
                        print(expendableItem);
                        slots[j, i].SetItem(expendableItem);
                    }
                    else
                    {
                        slots[j, i].SetItem(item);
                    }

                    JsonSave();
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

        width = inventoryStruct.width;                                      //�κ��丮�� ����
        height = inventoryStruct.height;                                    //�κ��丮�� ����

        slots = new Slot[width, height];                                    //������ ũ�� ����

        Transform parent = transform.Find("InventoryBackground");           //�κ��丮 Background�� �ڽ����� �ѰŴ�!

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


        JsonLoad();                                                         //���Ա��� �� ��� �ε� �ѹ� ����

        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                if (slots[i, j].assignedItem != null)
                {
                    Destroy(slots[i, j].assignedItem.gameObject);
                }
            }
        }
        gameObject.SetActive(false);    //�κ��丮�� ��Ȱ��ȭ ��Ű��

    }
}