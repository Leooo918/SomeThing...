using UnityEngine;

public class OpenInventory : MonoBehaviour
{
    public Inventory myInventory = null;   //이 스크립트가 열 인벤토리

    public string inventoryName = "MyInventory";  //이 인벤토리가 열 인벤토리의 이름

    private void Start()
    {
        for(int i = 0;i <  InventoryManager.instance.allInventoried.Count; i++)
        {
            if(inventoryName == InventoryManager.instance.allInventoried[i].inventoryName)
            {
                myInventory = InventoryManager.instance.allInventoried[i];
                InventoryClose();
            }
        }
    }

    private void Update()
    {
        #region 디버그용
        if (Input.GetKeyDown(KeyCode.C))
        {
            InventoryClose();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            InventoryOpen();
        }
        #endregion
    }

    public void InventoryOpen()
    {
        if (myInventory.gameObject.activeSelf == true) return;

        InventoryManager.instance.openInventoryList.Add(myInventory);   //인벤토리 메니저에 openInventory에 이 인벤토리를 추가
        myInventory.gameObject.SetActive(true);     //인벤토리를 활성화 시키고
        myInventory.JsonLoad();
    }

    public void InventoryClose()
    {
        if (myInventory.gameObject.activeSelf == false) return;

        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++) //인벤토리 매니저에 openInventory를 전부 돌며
        {
            if (InventoryManager.instance.openInventoryList[i].inventoryName == inventoryName)  //이 인벤토리와 이름이 같은 인벤토리가 있다면
            {
                InventoryManager.instance.openInventoryList.RemoveAt(i);    //그 인벤토리를 openInventory에서 뺀다.
            }
        }

        myInventory.JsonSave();

        for (int i = 0; i < myInventory.slots.GetLength(0); i++)
        {
            for (int j = 0; j < myInventory.slots.GetLength(1); j++)
            {
                if(myInventory.slots[i, j].assignedItem != null)
                {
                    Destroy(myInventory.slots[i, j].assignedItem.gameObject);
                }
            }
        }
        myInventory.gameObject.SetActive(false);    //인벤토리를 비활성화 시키고
    }
}
