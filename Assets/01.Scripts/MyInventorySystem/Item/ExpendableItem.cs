using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public abstract class ExpendableItem : Item, IPointerEnterHandler, IPointerExitHandler
{
    protected Expendable expendableItemStruct;                          //이 아이템의 정보를 가지고 있는 구조체

    private event Action<PointerEventData> switchAndCallAction = null;

    protected ItemDevider itemDevider = null;                           //아이템 나누는 놈

    protected int maxItemNum = 99;                                      //최대 들고 있을 수 있는 아이템의 수
    protected int reuseCoolTime = 1;                                    //아이템의 재사용 쿨타임
    protected int currentReusesCoolTime = 0;
    public int currentItemNum = 60;                                     //현재 아이템 개수

    private bool readyCombineItem = false;                              //아이템이 합칠 준비가 되었나(합칠 수 있는 아이템을 들고 있는 상태에서, 이 아이템 위에 마우스 포인터가 올라가 있나)


    private TextMeshProUGUI itemNumText = null;                         //아이템 개수 텍스트

    private bool isReadyDevide = false;                                 //나눌 준비가 됬는가
    private bool isAccepted = false;                                    //나눌 때 수락을 했는가
    private bool isCanceled = false;                                    //나눌 때 취소를 했는가


    private int itemValue = 0;

    #region 프로퍼티
    public bool ReadyCombineItem => readyCombineItem;
    public int CurrentItemNum => currentItemNum;
    public int MaxitemNum => maxItemNum;
    public bool IsReadyDevide => isReadyDevide;
    public int ItemValue => itemValue;
    #endregion


    protected override void Awake()
    {
        base.Awake();

        itemNumText = transform.Find("Text").GetComponent<TextMeshProUGUI>();   //아이템 개수 지정
    }



    protected override void Update()
    {
        base.Update();

        if (currentItemNum <= 0)
        {
            Destroy(gameObject);
        }
    }

    //사라질 때 액션 빼주기
    private void OnDisable()
    {
        itemDevider.acceptAction -= Devide;      //사라질 때 액션 빼주기
        switchAndCallAction -= base.OnPointerUp;
    }

    private void OnDestroy()
    {
        itemDevider.acceptAction -= Devide;
        switchAndCallAction -= base.OnPointerUp;
    }

    private void OnApplicationQuit()
    {
        itemDevider.acceptAction -= Devide;      //사라질 때 액션 빼주기
        switchAndCallAction -= base.OnPointerUp;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        //현재 아이템을 들고 있고 그 아이템이 같은 아이템이라면 합칠 준비를 한다
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
        readyCombineItem = false;   //합치지 마라
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
            for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)                             //현재 열려있는 모든 인벤토리의 슬롯을 확인하며
            {
                for (int j = 0; j < InventoryManager.instance.openInventoryList[i].slots.GetLength(0); j++)
                {
                    for (int k = 0; k < InventoryManager.instance.openInventoryList[i].slots.GetLength(1); k++)
                    {
                        //그 슬롯이 아이템을 넣을 준비가 된 슬롯이라면
                        if (InventoryManager.instance.openInventoryList[i].slots[j, k].IsSelected && lastSlot.Contains(InventoryManager.instance.openInventoryList[i].slots[j, k]) == false)
                        {
                            InventoryManager.instance.openInventoryList[i].slots[j, k].assignedItem = this;         //그 슬롯에 아이템을 넣어주고
                            InventoryManager.instance.openInventoryList[i].slots[j, k].SetPosition(this, true);     //위치를 찾아줌(근데 지난 위치에 바뀐 위치를 지난 위치라고 지정해주지는 않음)
                            assignedSlot.Add(InventoryManager.instance.openInventoryList[i].slots[j, k]);           //현재 이 아이템에 지정된 아이템은 이놈 들이다!
                            isChecked = true;
                        }
                        InventoryManager.instance.openInventoryList[i].slots[j, k].ResetColor();                    //슬롯들 색 초기화 해주고
                    }
                }
            }

            if (isChecked)                                                                                          //아이템이 지정 되었었다면
                StartCoroutine(DevideItemCoroutine(switchAndCallAction, eventData));                                //아이템 나누는 코루틴 실행(아이템 나누는 팝업창을 뛰어준다.)
            else
                base.OnPointerUp(eventData);                                                                        //아니면 그냥 하던대로
        }
        else
        {
            base.OnPointerUp(eventData);
        }

    }



    public virtual void CombineItem(ExpendableItem item) //여기서 받는 아이템은 현재 들고있는 아이템(옮길거) this는 옮겨질 위치
    {
        if (item.CurrentItemNum + CurrentItemNum <= MaxitemNum) //만약 아이템 1칸에 소지할 수 있는 최대 아이템 개수를 넘어서지 않았다면 두개를 합친다
        {
            currentItemNum += item.CurrentItemNum;
            Destroy(item.gameObject);
        }
        else                                                    //그렇지 않다면 하나를 최대개수로 하고 나머지 아이템 수 만큼 들고있는 아이템에 넘겨 지난 위치로 옮긴다. 
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
        isAccepted = true;                                  //수락했다!

        ExpendableItem copiedItem = Instantiate(expendableItemStruct.pfItem, transform.parent).GetComponent<ExpendableItem>();  //아이템을 하나 새로 만들어 주고


        for (int i = 0; i < lastSlot.Count; i++)
        {
            lastSlot[i].CheckSlot(copiedItem);
            lastSlot[i].Inventory.slots[lastSlot[i].OriginPos.x, lastSlot[i].OriginPos.y].SetIsOriginPos();
            lastSlot[i].assignedItem = copiedItem;          //이전 슬롯에 새로 클론한 아이템을 넣고
            copiedItem.assignedSlot.Add(lastSlot[i]);       //새로 클론한 아이템의 현재 할당되있는 슬롯에 이전 슬롯을 추가해줌
            copiedItem.lastSlot.Add(lastSlot[i]);
        }

        copiedItem.Init(expendableItemStruct, currentItemNum - value, rectTransform.eulerAngles.z);
        copiedItem.SetPosition(lastPosition);               //새 아이템의 위치를 잡고 초기화 해주고 개수를 지정해주고 텍스트를 바꺼줌
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
        DevideItem();                               //아이템 나누는 놈을 켜준다
        yield return new WaitUntil(() => isAccepted == true || isCanceled == true);

        if (isCanceled == true)                     //취소됬다면
        {
            CancelDevide();                         //아이템을 이전 위치로 보냄
        }

        if (isAccepted == true)                     //수락됬다면
        {
            func?.Invoke(eventData);                //Devide함수 실행, 하고 나머지는 하던데로

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

        itemDevider = InventoryManager.instance.ItemDevider;    //아이템 나눌 때 뛰우는 팝업창 들고옴

        if (currentItemNum <= 0)
        {
            Destroy(gameObject);
        }

        UpdateText();
    }
}