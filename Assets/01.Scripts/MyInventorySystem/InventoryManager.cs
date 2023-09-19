using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance = null;     //게임 메니저 만들고 바꿔

    //일단 인스펙터 창에서 넣고 나중에 게임메니저 만들고 Addressable이나 Resource.Load로 가져오기
    public InventorySO inventorySO = null;  
    public ItemSO itemSO = null;    

    public List<Inventory> openInventoryList = new List<Inventory>();   //현재 열려있는 인벤토리들
    public Item currentItem = null;     //현재 플레이어가 드래그하고 있는 아이템
    public bool isDeviding = false;     //아이템을 나누는 중인지 확인

    private ItemDevider itemDevidrer = null;    //아이템을 배분할 때 쓰이는 팝업창
    
    #region 프로퍼티
    public ItemDevider ItemDevider => itemDevidrer;
    #endregion

    public void Init(InventorySO inventorySO, ItemSO itemSO, ItemDevider itemDevidrer)
    {
        this.inventorySO = inventorySO;
        this.itemSO = itemSO;
        this.itemDevidrer = itemDevidrer;
        itemDevidrer.gameObject.SetActive(false);   //찾고 비활성화 시켜둬

        for(int i = 0; i < inventorySO.inventoryStructs.Length; i++)
        {
            Inventory inventory = Instantiate(inventorySO.inventoryStructs[i].pfInvnetory).GetComponent<Inventory>();
            inventory.transform.SetParent(GameObject.Find("Canvas").transform);

            RectTransform rect = inventory.GetComponent<RectTransform>();
            rect.localScale = new Vector3(1, 1, 1);
            rect.anchoredPosition3D = new Vector3(0, 0, 0);

            inventory.transform.SetSiblingIndex(1);
            inventory.Init(itemSO,inventorySO, inventorySO.inventoryStructs[i]);
        }
    }
}
