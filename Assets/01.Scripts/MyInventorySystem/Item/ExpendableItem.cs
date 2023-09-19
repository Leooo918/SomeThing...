using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public abstract class ExpendableItem : Item, IPointerEnterHandler, IPointerExitHandler
{
    protected Expendable expendableItemStruct;                          //�� �������� ������ ������ �ִ� ����ü

    private event Action<PointerEventData> switchAndCallAction = null;

    protected ItemDevider itemDevider = null;                           //������ ������ ��

    protected int maxItemNum = 99;                                      //�ִ� ��� ���� �� �ִ� �������� ��
    protected int reuseCoolTime = 1;                                    //�������� ���� ��Ÿ��
    protected int currentReusesCoolTime = 0;
    public int currentItemNum = 60;                                     //���� ������ ����

    private bool readyCombineItem = false;                              //�������� ��ĥ �غ� �Ǿ���(��ĥ �� �ִ� �������� ��� �ִ� ���¿���, �� ������ ���� ���콺 �����Ͱ� �ö� �ֳ�)


    private TextMeshProUGUI itemNumText = null;                         //������ ���� �ؽ�Ʈ

    private bool isReadyDevide = false;                                 //���� �غ� ��°�
    private bool isAccepted = false;                                    //���� �� ������ �ߴ°�
    private bool isCanceled = false;                                    //���� �� ��Ҹ� �ߴ°�


    private int itemValue = 0;

    #region ������Ƽ
    public bool ReadyCombineItem => readyCombineItem;
    public int CurrentItemNum => currentItemNum;
    public int MaxitemNum => maxItemNum;
    public bool IsReadyDevide => isReadyDevide;
    public int ItemValue => itemValue;
    #endregion


    protected override void Awake()
    {
        base.Awake();

        itemNumText = transform.Find("Text").GetComponent<TextMeshProUGUI>();   //������ ���� ����
    }



    protected override void Update()
    {
        base.Update();

        if (currentItemNum <= 0)
        {
            Destroy(gameObject);
        }
    }

    //����� �� �׼� ���ֱ�
    private void OnDisable()
    {
        itemDevider.acceptAction -= Devide;      //����� �� �׼� ���ֱ�
        switchAndCallAction -= base.OnPointerUp;
    }

    private void OnDestroy()
    {
        itemDevider.acceptAction -= Devide;
        switchAndCallAction -= base.OnPointerUp;
    }

    private void OnApplicationQuit()
    {
        itemDevider.acceptAction -= Devide;      //����� �� �׼� ���ֱ�
        switchAndCallAction -= base.OnPointerUp;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        //���� �������� ��� �ְ� �� �������� ���� �������̶�� ��ĥ �غ� �Ѵ�
        if (InventoryManager.instance.currentItem == null || InventoryManager.instance.isDeviding == true) return;

        if (ItemName == InventoryManager.instance.currentItem.ItemName)
        {
            transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

            readyCombineItem = true;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true) return;

        transform.localScale = new Vector3(1f, 1f, 1f);
        readyCombineItem = false;   //��ġ�� ����
    }



    public override void OnPointerUp(PointerEventData eventData)
    {
        bool isChecked = false;

        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true) return;

        isAccepted = false;
        isCanceled = false;
        switchAndCallAction += base.OnPointerUp;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)                             //���� �����ִ� ��� �κ��丮�� ������ Ȯ���ϸ�
            {
                for (int j = 0; j < InventoryManager.instance.openInventoryList[i].slots.GetLength(0); j++)
                {
                    for (int k = 0; k < InventoryManager.instance.openInventoryList[i].slots.GetLength(1); k++)
                    {
                        //�� ������ �������� ���� �غ� �� �����̶��
                        if (InventoryManager.instance.openInventoryList[i].slots[j, k].IsSelected && lastSlot.Contains(InventoryManager.instance.openInventoryList[i].slots[j, k]) == false)
                        {
                            InventoryManager.instance.openInventoryList[i].slots[j, k].assignedItem = this;         //�� ���Կ� �������� �־��ְ�
                            InventoryManager.instance.openInventoryList[i].slots[j, k].SetPosition(this, true);     //��ġ�� ã����(�ٵ� ���� ��ġ�� �ٲ� ��ġ�� ���� ��ġ��� ������������ ����)
                            assignedSlot.Add(InventoryManager.instance.openInventoryList[i].slots[j, k]);           //���� �� �����ۿ� ������ �������� �̳� ���̴�!
                            isChecked = true;
                        }
                        InventoryManager.instance.openInventoryList[i].slots[j, k].ResetColor();                    //���Ե� �� �ʱ�ȭ ���ְ�
                    }
                }
            }

            if (isChecked)                                                                                          //�������� ���� �Ǿ����ٸ�
                StartCoroutine(DevideItemCoroutine(switchAndCallAction, eventData));                                //������ ������ �ڷ�ƾ ����(������ ������ �˾�â�� �پ��ش�.)
            else
                base.OnPointerUp(eventData);                                                                        //�ƴϸ� �׳� �ϴ����
        }
        else
        {
            base.OnPointerUp(eventData);
        }

    }



    public virtual void CombineItem(ExpendableItem item) //���⼭ �޴� �������� ���� ����ִ� ������(�ű��) this�� �Ű��� ��ġ
    {
        if (item.CurrentItemNum + CurrentItemNum <= MaxitemNum) //���� ������ 1ĭ�� ������ �� �ִ� �ִ� ������ ������ �Ѿ�� �ʾҴٸ� �ΰ��� ��ģ��
        {
            currentItemNum += item.CurrentItemNum;
            Destroy(item.gameObject);
        }
        else                                                    //�׷��� �ʴٸ� �ϳ��� �ִ밳���� �ϰ� ������ ������ �� ��ŭ ����ִ� �����ۿ� �Ѱ� ���� ��ġ�� �ű��. 
        {
            SetPosition(lastPosition);
            item.currentItemNum = item.currentItemNum + currentItemNum - maxItemNum;
            currentItemNum = maxItemNum;
            item.UpdateText();
        }
        UpdateText();
    }



    private void Devide(int value)
    {
        isAccepted = true;                                  //�����ߴ�!

        ExpendableItem copiedItem = Instantiate(expendableItemStruct.pfItem, transform.parent).GetComponent<ExpendableItem>();  //�������� �ϳ� ���� ����� �ְ�


        for (int i = 0; i < lastSlot.Count; i++)
        {
            lastSlot[i].CheckSlot(copiedItem);
            lastSlot[i].Inventory.slots[lastSlot[i].OriginPos.x, lastSlot[i].OriginPos.y].SetIsOriginPos();
            lastSlot[i].assignedItem = copiedItem;          //���� ���Կ� ���� Ŭ���� �������� �ְ�
            copiedItem.assignedSlot.Add(lastSlot[i]);       //���� Ŭ���� �������� ���� �Ҵ���ִ� ���Կ� ���� ������ �߰�����
            copiedItem.lastSlot.Add(lastSlot[i]);
        }

        copiedItem.Init(expendableItemStruct, currentItemNum - value, rectTransform.eulerAngles.z);
        copiedItem.SetPosition(lastPosition);               //�� �������� ��ġ�� ��� �ʱ�ȭ ���ְ� ������ �������ְ� �ؽ�Ʈ�� �ٲ���
        copiedItem.Setup();
        copiedItem.UpdateText();


        lastSlot.Clear();
        for (int i = 0; i < assignedSlot.Count; i++)
        {
            lastSlot.Add(assignedSlot[i]);
        }
        lastPosition = rectTransform.anchoredPosition;
        lastRotation = rectTransform.eulerAngles.z;

        currentItemNum = value;

        UpdateText();
    }


    IEnumerator DevideItemCoroutine(Action<PointerEventData> func, PointerEventData eventData)
    {
        DevideItem();                               //������ ������ ���� ���ش�
        yield return new WaitUntil(() => isAccepted == true || isCanceled == true);

        if (isCanceled == true)                     //��҉�ٸ�
        {
            CancelDevide();                         //�������� ���� ��ġ�� ����
        }

        if (isAccepted == true)                     //�����ٸ�
        {
            func?.Invoke(eventData);                //Devide�Լ� ����, �ϰ� �������� �ϴ�����

            for (int i = 0; i < assignedSlot.Count; i++)
            {
                for (int j = 0; j < assignedSlot.Count; j++)
                {
                    if (assignedSlot[j] == assignedSlot[i])
                    {
                        assignedSlot.RemoveAt(j);
                    }
                }
            }
        }
    }


    public virtual void DevideItem()
    {
        itemDevider.gameObject.SetActive(true);
        itemDevider.item = this;
        itemDevider.acceptAction += Devide;
        itemDevider.Init(currentItemNum, itemName);
    }


    public void CancelDevide()
    {
        SetPosition(lastPosition);
        for (int i = 0; i < assignedSlot.Count; i++)
        {
            assignedSlot[i].assignedItem = null;
        }
        assignedSlot.Clear();
        for (int i = 0; i < lastSlot.Count; i++)
        {
            lastSlot[i].assignedItem = this;
            assignedSlot.Add(lastSlot[i]);
        }
        Setup();
    }


    public void CancelDeviding()
    {
        isCanceled = true;
    }


    public virtual void UseItem(int value)
    {
        currentItemNum -= value;

        if (currentItemNum <= 0)
        {
            gameObject.SetActive(false);
        }
    }


    public void UpdateText()
    {
        itemNumText.SetText(currentItemNum.ToString());
    }

    public void SetCurrentItemNum(int value)
    {
        currentItemNum = value;
    }

    public void Init(Expendable expendableItemStruct, int currentItemNum, float rotation)
    {
        this.expendableItemStruct = expendableItemStruct;

        Vector3 rot = new Vector3(0, 0, rotation);

        rectTransform.eulerAngles = rot;

        if (rotation % 180 != 0)
        {
            int temp = this.expendableItemStruct.size.x;
            this.expendableItemStruct.size.x = this.expendableItemStruct.size.y;
            this.expendableItemStruct.size.y = temp;

        }

        maxItemNum = this.expendableItemStruct.maxItemNum;
        itemValue = this.expendableItemStruct.itemValue;
        this.currentItemNum = currentItemNum;

        sizeX = this.expendableItemStruct.size.x;
        sizeY = this.expendableItemStruct.size.y;

        lastRotation = rectTransform.eulerAngles.z;
        lastPosition = rectTransform.anchoredPosition;

        for (int i = 0; i < assignedSlot.Count; i++)
        {
            lastSlot.Add(assignedSlot[i]);
        }

        itemDevider = InventoryManager.instance.ItemDevider;    //������ ���� �� �ٿ�� �˾�â ����

        if (currentItemNum <= 0)
        {
            Destroy(gameObject);
        }

        UpdateText();
    }
}