using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerWeaponSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Weapon assignedWeapon = null;
    private WeaponSO weaponSO = null;
    private ItemSO itemSO = null;
    private Item item = null;
    private Transform weaponParent = null;
    private PlayerStatus status = null;
    private Inventory myInventory = null;
    private bool isSelected = false;

    [SerializeField] private float doubleClickTime = 0.2f;
    private float doubleClickTimeDown = 0f;

    [SerializeField] private int slotNum = 0;

    private void Update()
    {
        if (doubleClickTimeDown >= 0)
        {
            doubleClickTimeDown -= Time.deltaTime;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isSelected = true;
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isSelected = false;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public bool SetWeapon(Item item, bool isLoading = false)
    {
        if (isSelected == false && isLoading == false) return false;

        if (item.ItemType == ItemType.weapons)
        {
            for (int i = 0; i < weaponSO.weapons.Length; i++)
            {
                if (weaponSO.weapons[i].name == item.itemName)
                {
                    assignedWeapon = Instantiate(weaponSO.weapons[i].weapon, weaponParent).GetComponent<Weapon>();
                    status.mountingWeapon[slotNum] = assignedWeapon;
                    transform.Find("Frame/Sprite").GetComponent<Image>().sprite = weaponSO.weapons[i].weaponImage;
                    transform.Find("Frame/Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    assignedWeapon.gameObject.SetActive(false);
                    status.OnChangeWeapon(status.CurWeaponNum);

                    for (int j = 0; j < itemSO.items.Count; j++)
                    {
                        if (itemSO.items[j].itemName == item.itemName)
                        {
                            this.item = Instantiate(item, GameObject.Find("Canvas").transform);
                            RectTransform r = item.GetComponent<RectTransform>();
                            r.localPosition = new Vector3(1, 1, 1);
                            r.anchoredPosition3D = new Vector3(0, 0, 0);
                            this.item.Init(itemSO.items[j], 0); ;
                            this.item.gameObject.SetActive(false);
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public void DeleteWeapon()
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

        Destroy(assignedWeapon.gameObject);
        assignedWeapon = null;
        transform.Find("Frame/Sprite").GetComponent<Image>().sprite = null;
        transform.Find("Frame/Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (doubleClickTimeDown <= 0)
            {
                doubleClickTimeDown = doubleClickTime;
                return;
            }

            DeleteWeapon();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            UIManager.instance.UnEquip(eventData.position, DeleteWeapon);
        }
    }

    public void Init(WeaponSO weaponSO, ItemSO itemSO, Transform weaponParent, PlayerStatus status)
    {
        this.weaponSO = weaponSO;
        this.itemSO = itemSO;
        this.weaponParent = weaponParent;
        this.status = status;
    }
}