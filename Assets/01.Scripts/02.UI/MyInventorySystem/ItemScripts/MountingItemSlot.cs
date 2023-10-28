using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class MountingItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected Item item = null;
    protected ItemSO itemSO = null;

    [SerializeField] private float doubleClickTime = 0.2f;
    private float doubleClickTimeDown = 0f;

    protected bool isSelected = false;

    public Item assignedItem => item;

    protected virtual void Update()
    {
        if (doubleClickTimeDown >= 0)
        {
            doubleClickTimeDown -= Time.deltaTime;
        }
    }

    public abstract bool SetItem(Item item, float durability, bool isLoading = false);

    public virtual void UnEquip()
    {
        UIManager.instance.UnEquipUI.gameObject.SetActive(false);
        if (item == null) return;

        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)
        {
            if (InventoryManager.instance.openInventoryList[i].SetItem(item) == true)
            {
                item.gameObject.SetActive(true);
                break;
            }
        }

        transform.Find("Frame/Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        GameManager.instance.Save();
        item = null;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
        {
            isSelected = true;
        }
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isSelected = false;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (doubleClickTimeDown <= 0)
            {
                doubleClickTimeDown = doubleClickTime;
                return;
            }

            UnEquip();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            UIManager.instance.UnEquip(eventData.position, UnEquip);
        }
    }
}
