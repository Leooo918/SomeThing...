using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponSlot : MountingItemSlot
{
    private Weapon assignedWeapon = null;
    private WeaponSO weaponSO = null;
    private Transform weaponParent = null;
    private PlayerStatus status = null;

    [SerializeField] private int slotNum = 0;

    public override bool SetItem(Item item, float durability, bool isLoading = false)
    {
        if (isSelected == false && isLoading == false) return false;

        if (item.TryGetComponent<ItemWeapon>(out ItemWeapon itemWepon))
        {
            for (int i = 0; i < weaponSO.weapons.Length; i++)
            {
                if (weaponSO.weapons[i].name == item.itemName)
                {
                    assignedWeapon = Instantiate(weaponSO.weapons[i].weapon, weaponParent).GetComponent<Weapon>();
                    assignedWeapon.Init(durability);

                    status.mountingWeapon[slotNum] = assignedWeapon;

                    transform.Find("Frame/Sprite").GetComponent<Image>().sprite = weaponSO.weapons[i].weaponImage;
                    transform.Find("Frame/Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 1);

                    assignedWeapon.gameObject.SetActive(false);
                    status.OnChangeWeapon();

                    for (int j = 0; j < itemSO.items.Count; j++)
                    {
                        if (itemSO.items[j].itemName == item.itemName)
                        {
                            this.item = Instantiate(item, GameObject.Find("Canvas").transform);

                            RectTransform r = item.GetComponent<RectTransform>();

                            r.localPosition = new Vector3(1, 1, 1);
                            r.anchoredPosition3D = new Vector3(0, 0, 0);

                            this.item.Init(itemSO.items[j], 0);
                            this.item.GetComponent<ItemWeapon>().Init(durability);
                            this.item.gameObject.SetActive(false);
                        }
                    }

                    GameManager.instance.Save();
                    return true;
                }
            }
        }
        return false;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        Destroy(assignedWeapon.gameObject);

        assignedWeapon = null;
        transform.Find("Frame/Sprite").GetComponent<Image>().sprite = null;
    }

    public void UnEquipWithOutDestroyGameObject()
    {
        if (item == null) return;

        //for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)
        //{
        //    if (InventoryManager.instance.openInventoryList[i].SetItem(item) == true)
        //    {
        //        item.gameObject.SetActive(true);
        //        break;
        //    }
        //}

        item = null;

        assignedWeapon = null;
        //GameManager.instance.Save();
        transform.Find("Frame/Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        transform.Find("Frame/Sprite").GetComponent<Image>().sprite = null;
    }

    public void Init(WeaponSO weaponSO, ItemSO itemSO, Transform weaponParent, PlayerStatus status)
    {
        this.weaponSO = weaponSO;
        this.itemSO = itemSO;
        this.weaponParent = weaponParent;
        this.status = status;
    }
}