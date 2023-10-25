using UnityEngine;
using UnityEngine.UI;

public class PlayerLanternSlot : MountingItemSlot
{
    public override bool SetItem(Item item,float durability, bool isLoading = false)
    {
        if (isSelected == false && isLoading == false) return false;

        if (item.TryGetComponent<Lantern>(out Lantern lantern))
        {
            transform.Find("Frame/Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 1);
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
            return true;
        }
        return false;
    }

    public Item GetItem()
    {
        return item;
    }

    public void Init(ItemSO itemSO)
    {
        this.itemSO = itemSO;
    }
}
