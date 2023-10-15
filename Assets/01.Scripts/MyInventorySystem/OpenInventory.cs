using UnityEngine;

public class OpenInventory : MonoBehaviour
{
    public Inventory myInventory = null;   //�� ��ũ��Ʈ�� �� �κ��丮

    public string inventoryName = "MyInventory";  //�� �κ��丮�� �� �κ��丮�� �̸�

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
        #region ����׿�
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

        InventoryManager.instance.openInventoryList.Add(myInventory);   //�κ��丮 �޴����� openInventory�� �� �κ��丮�� �߰�
        myInventory.gameObject.SetActive(true);     //�κ��丮�� Ȱ��ȭ ��Ű��
        myInventory.JsonLoad();
    }

    public void InventoryClose()
    {
        if (myInventory.gameObject.activeSelf == false) return;

        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++) //�κ��丮 �Ŵ����� openInventory�� ���� ����
        {
            if (InventoryManager.instance.openInventoryList[i].inventoryName == inventoryName)  //�� �κ��丮�� �̸��� ���� �κ��丮�� �ִٸ�
            {
                InventoryManager.instance.openInventoryList.RemoveAt(i);    //�� �κ��丮�� openInventory���� ����.
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
        myInventory.gameObject.SetActive(false);    //�κ��丮�� ��Ȱ��ȭ ��Ű��
    }
}
