using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isMouseOn = false;

    private Vector2Int originPos;
    public Vector2Int OriginPos => originPos;

    private bool isSelected = false;
    private bool isItemAssignedByItemSize = false;
    private bool isAssignedItemsOriginPos = false;

    public Item assignedItem = null;
    private Image image = null;
    private Inventory inventory = null;
    private Shop shop = null;

    private int posX, posY;


    public int PosX => posX;
    public int PosY => posY;
    public bool IsSelected => isSelected;
    public bool IsAssignedItemsOriginPos => isAssignedItemsOriginPos;
    public Inventory Inventory => inventory;

    private void Awake()
    {
        image = GetComponent<Image>();
    }


    private void Update()
    {
        if (assignedItem == null)
        {
            isAssignedItemsOriginPos = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) //마우스 버튼이 올라갔을 때
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || inventory == null) return;

        SoundManager.instance.PlayOneShot(Sound.CursurMoveSound);

        isMouseOn = true;
        if (InventoryManager.instance.currentItem == null)
        {
            if (assignedItem == null)
            {
                image.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            }
        }
        else
        {
            isItemAssignedByItemSize = CheckSlot(InventoryManager.instance.currentItem); //여긴 문제 없을 걸(시작점 찾고 아이템을 둘 수 있는지 확인 하는 메서드)

            SetColorByItemSize(isItemAssignedByItemSize);
        }
    }

    public void OnPointerExit(PointerEventData eventData) // 여기에 문제가 존나 많다
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || inventory == null) return;

        isMouseOn = false;
        Item item = InventoryManager.instance.currentItem;

        if (InventoryManager.instance.currentItem == null)
        {
            ResetColor(); //이건 작동했음
        }
        else
        {
            for (int i = 0; i < item.SizeX; i++)
            {
                for (int j = 0; j < item.SizeY; j++)
                {
                    if (originPos.x + i >= inventory.Width || originPos.y + j >= inventory.Height || originPos.x + i < 0 || originPos.y + j < 0) return;
                    inventory.slots[originPos.x + i, originPos.y + j].ResetColor();                             //색을  1,1,1,1로 봐꺼주고
                    inventory.slots[originPos.x + i, originPos.y + j].isSelected = false;                       //isSelect를 false로 바꺼준
                }
            }
        }

        if (InventoryManager.instance.currentItem != null)
        {
            if (InventoryManager.instance.currentItem.GetComponent<ExpendableItem>() != null)
            {
                if (InventoryManager.instance.currentItem.GetComponent<ExpendableItem>().IsReadyDevide)
                {
                    return;
                }
            }
        }


        isSelected = false;

    }

    public void ResetColor()
    {
        image.color = new Color(1f, 1f, 1f, 1f);
        if (InventoryManager.instance.currentItem != null)
        {
            if (InventoryManager.instance.currentItem.GetComponent<ExpendableItem>() != null)
            {
                if (InventoryManager.instance.currentItem.GetComponent<ExpendableItem>().IsReadyDevide)
                {
                    return;
                }
            }
        }
        isSelected = false;
    }

    public void SetPosition(Item item, bool isSetOnlyPositoin = false, bool isSettingItem = false)
    {
        if (isSettingItem == true)                                  //아이템을 셋팅 중이라면
        {
            originPos = new Vector2Int(posX, posY);                 //이 슬롯이 아이템의 시작 위치다
        }

        if (isMouseOn == false && isSettingItem == false) return;


        RectTransform inventoryRectTransform = inventory.transform.Find("InventoryBackground").GetComponent<RectTransform>();

        //기본 인벤토리 크기보다 얼마나 작은지
        int a = inventory.Width - 7;
        int b = inventory.Height - 9;

        Vector2 position = new Vector2(inventoryRectTransform.anchoredPosition.x + 600f - (a * 50), inventoryRectTransform.anchoredPosition.y + 100f - (b * 50));


        if (item.SizeX == 1)
        {
            position += new Vector2(originPos.x * 100 + 60, 0);
        }
        else
        {
            if (item.SizeX % 2 == 0)
            {
                position += new Vector2((originPos.x + item.SizeX / 2 + 1) * 100 - 90, 0);
            }
            else
            {
                position += new Vector2((originPos.x + item.SizeX / 2) * 100 + 60, 0);
            }
        }


        if (item.SizeY == 1)
        {
            position += new Vector2(0, originPos.y * 100 + 40);
        }
        else
        {
            if (item.SizeY % 2 == 0)
            {
                position += new Vector2(0, (originPos.y + item.SizeY / 2) * 100 - 10);
            }
            else
            {
                position += new Vector2(0, (originPos.y + item.SizeY / 2) * 100 + 40);
            }
        }


        inventory.slots[originPos.x, originPos.y].isAssignedItemsOriginPos = true;


        if (isSetOnlyPositoin == false)
        {
            item.SetPosition(position);
        }
        else
        {
            item.SetOnlyPosition(position);
        }
    }

    public bool CheckSlot(Item item)    //아이템 사이즈에 맞춰 그 칸에 들어갈 수 있나 확인   
    {
        if (item.ClickPointInt.x > 0)
            originPos.x = posX - (item.SizeX - item.ClickPointInt.x);
        else if (item.ClickPointInt.x < 0)
            originPos.x = posX + (item.SizeX / 2 - Mathf.Abs(item.ClickPointInt.x));
        else
            originPos.x = posX - (item.SizeX / 2);

        if (item.ClickPointInt.y > 0)
            originPos.y = posY - (item.SizeY - item.ClickPointInt.y);
        else if (item.ClickPointInt.y < 0)
            originPos.y = posY - (item.SizeY / 2 - Mathf.Abs(item.ClickPointInt.y));
        else
            originPos.y = posY - (item.SizeY / 2);


        if (originPos.x < 0) originPos.x = 0;
        if (originPos.y < 0) originPos.y = 0;

        if (originPos.x + item.SizeX >= inventory.Width) originPos.x = inventory.Width - item.SizeX;
        if (originPos.y + item.SizeY >= inventory.Height) originPos.y = inventory.Height - item.SizeY;


        for (int i = 0; i < item.SizeX; i++)
        {
            for (int j = 0; j < item.SizeY; j++)
            {
                if (inventory.slots[originPos.x + i, originPos.y + j].assignedItem != null)
                    return false;
            }
        }

        return true;
    }

    private void SetColorByItemSize(bool isNotAssigned)
    {
        Item item = InventoryManager.instance.currentItem;

        for (int i = 0; i < item.SizeX; i++)
        {
            for (int j = 0; j < item.SizeY; j++)
            {
                inventory.slots[originPos.x + i, originPos.y + j].ColorSlot(isNotAssigned);
            }
        }
    }

    public void ColorSlot(bool isNotAssigned)
    {
        if (InventoryManager.instance.currentItem != null)
        {
            if (InventoryManager.instance.currentItem.GetComponent<ExpendableItem>() != null)
            {
                if (InventoryManager.instance.currentItem.GetComponent<ExpendableItem>().IsReadyDevide)
                {
                    return;
                }
            }
        }

        if (isNotAssigned == true)
        {
            image.color = new Color(0f, 0.8f, 0.25f, 1f);
            isSelected = true;
        }
        else
        {
            image.color = new Color(0.8f, 0f, 0f, 1f);
            isSelected = false;
        }
    }

    public void SetItem(Item item)
    {
        isAssignedItemsOriginPos = true;
        SetPosition(item, false, true);                                     //위치를 지정해줘

        for (int i = 0; i < item.assignedSlot.Count; i++)
        {
            item.assignedSlot[i].assignedItem = null;
        }
        item.assignedSlot.Clear();


        for (int i = 0; i < item.SizeX; i++)
        {
            for (int j = 0; j < item.SizeY; j++)
            {
                if (posX + i >= inventory.Width || posY + j >= inventory.Height)
                {
                    Destroy(item.gameObject);
                    return;
                }
                item.assignedSlot.Add(inventory.slots[posX + i, posY + j]); //슬롯이랑 아이템에 그 아이템과 슬롯 지정
                inventory.slots[posX + i, posY + j].assignedItem = item;
            }
        }
    }

    public bool CheckCanSetPosition(Item item)
    {
        if (item == null) return false;

        if (posX + item.SizeX - 1 >= inventory.Width || posY + item.SizeY - 1 >= inventory.Height) //아이템이 인벤토리 밖으로 나가면
        {
            return false;
        }

        for (int i = 0; i < item.SizeX; i++)
        {
            for (int j = 0; j < item.SizeY; j++)
            {
                if (inventory.slots[posX + i, posY + j].assignedItem != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void SetIsOriginPos()
    {
        isAssignedItemsOriginPos = true;
    }

    public void Init(int posX, int posY, Inventory inventory)
    {
        this.posX = posX;
        this.posY = posY;

        this.inventory = inventory;
    }
}