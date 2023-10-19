using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerWeaponSlot : MountingItemSlot
{
    private Weapon assignedWeapon = null;
    private WeaponSO weaponSO = null;
    private Transform weaponParent = null;
    private PlayerStatus status = null;

    [SerializeField] private int slotNum = 0;

    public override bool SetItem(Item item, float durability, float proficiency, bool isLoading = false)
    {
        if (isSelected == false && isLoading == false) return false;

        if (item.TryGetComponent<ItemWeapon>(out ItemWeapon itemWepon))
        {
            for (int i = 0; i < weaponSO.weapons.Length; i++)
            {
                if (weaponSO.weapons[i].name == item.itemName)
                {
                    assignedWeapon = Instantiate(weaponSO.weapons[i].weapon, weaponParent).GetComponent<Weapon>();
                    assignedWeapon.Init(durability, proficiency);

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
                            this.item.Init(itemSO.items[j], 0);
                            this.item.gameObject.SetActive(false);
                        }
                    }
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

    public void Init(WeaponSO weaponSO, ItemSO itemSO, Transform weaponParent, PlayerStatus status)
    {
        this.weaponSO = weaponSO;
        this.itemSO = itemSO;
        this.weaponParent = weaponParent;
        this.status = status;
    }
}