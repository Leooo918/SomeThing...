using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance = null;     //���� �޴��� ����� �ٲ�

    //�ϴ� �ν����� â���� �ְ� ���߿� ���Ӹ޴��� ����� Addressable�̳� Resource.Load�� ��������
    public InventorySO inventorySO = null;  
    public ItemSO itemSO = null;    

    public List<Inventory> openInventoryList = new List<Inventory>();   //���� �����ִ� �κ��丮��
    public Item currentItem = null;     //���� �÷��̾ �巡���ϰ� �ִ� ������
    public bool isDeviding = false;     //�������� ������ ������ Ȯ��

    private ItemDevider itemDevidrer = null;    //�������� ����� �� ���̴� �˾�â
    
    #region ������Ƽ
    public ItemDevider ItemDevider => itemDevidrer;
    #endregion

    public void Init(InventorySO inventorySO, ItemSO itemSO, ItemDevider itemDevidrer)
    {
        this.inventorySO = inventorySO;
        this.itemSO = itemSO;
        this.itemDevidrer = itemDevidrer;
        itemDevidrer.gameObject.SetActive(false);   //ã�� ��Ȱ��ȭ ���ѵ�

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
