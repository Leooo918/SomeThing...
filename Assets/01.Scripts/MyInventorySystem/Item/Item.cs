using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ItemType
{
    weapons = 0,
    accessories = 1,
    ingrediants = 2,
    expendables = 3,
    ammo = 4,
    money = 5
}

public abstract class Item : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerClickHandler
{
    public string itemName = "";                            //아이템 이름

    protected ItemType itemType = new ItemType();
    private Vector2 clickPoint = Vector2.zero;              //클릭한 위치
    private Vector2Int clickPointInt = Vector2Int.zero;     //클릭한 위치와 아이템의 위치 오차

    protected int sizeX, sizeY;                             //아이템의 사이즈
    protected Normal normalItemStruct;


    protected RectTransform rectTransform = null;           //아이템 RectTransform
    protected Image image = null;                             //아이템 이미지
    protected Image[] backgroundImage;                        //클릭했을 때 나오는 배경
    protected Transform background = null;                    //배경 게임오브젝트 
    protected Camera mainCamera = null;                       //메인 카메라

    //회전을 위한 변수
    protected bool isItemSelected = false;                    //아이템 회전할 때 들고 있는 아이템만 회전시키기 위한 것

    //아이템의 저번 위치와 슬롯을 저장하는 놈
    public Vector2 lastPosition = Vector2.zero;             //아이템의 지난 위치
    public List<Slot> lastSlot = new List<Slot>();          //지난번에 저장되있던 모든 슬롯
    public float lastRotation = 0;


    public List<Slot> assignedSlot = new List<Slot>();      //현재 아이템이 포함되있는 모든 슬롯
    private PlayerWeaponSlot[] weaponSlots = null;

    #region 프로퍼티
    public Vector2Int ClickPointInt => clickPointInt;
    public string ItemName => itemName;
    public int SizeX => sizeX;
    public int SizeY => sizeY;
    public Normal NormalItemStruct => normalItemStruct;
    public ItemType ItemType => itemType;
    #endregion


    protected virtual void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        background = transform.Find("Background");
        image = transform.Find("Sprite").GetComponent<Image>();
        backgroundImage = background.GetComponentsInChildren<Image>();

        weaponSlots = FindObjectsByType<PlayerWeaponSlot>(FindObjectsSortMode.None);
    }

    protected virtual void Update()
    {
        CalculateClickedPoint();    //클릭한 위치와 아이템의 위치의 오차를 구함 (ex) x좌표로 -1칸, y좌표로 -1칸) clickPointInt에 저장됨
        RotateItem();               //아이템 회전
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;
        transform.Find("Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 1);

        UISoundManager.instance.PlayOneShot(Sound.ButtonClilckSound);         //소리함 기깔나게 나야지

        image.raycastTarget = false;

        for (int i = 0; i < assignedSlot.Count; i++)                        //현재 저장되어 있는 모든 슬롯을 초기화
        {
            assignedSlot[i].assignedItem = null;
        }
        assignedSlot.Clear();

        InventoryManager.instance.currentItem = this;                       //인벤토리 메니저에 현재 이 아이템을 들고 있다고 보고

        clickPoint = eventData.position - rectTransform.anchoredPosition;   //클릭한 위치를 이 아이템을 기준을로해서 받아옴

        //선택되면 아이템을 살짝 크게하고 뒤에 배경을 보이게 하고 알파값을 낮춘다
        Color c = image.color;
        c.a = 0.7f;
        image.color = c;
        background.gameObject.SetActive(true);
        transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

        isItemSelected = true;                                              //이 아이템은 현재 선택되었다
    }



    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;
        //드래그 할 떄 마우스 따라 움직이게
        rectTransform.anchoredPosition = eventData.position + new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize) - clickPoint;
    }


    public virtual void OnPointerUp(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;

        UISoundManager.instance.PlayOneShot(Sound.ButtonClilckSound);

        bool canSetPosition = false;
        Setup();                        //아이템 초기 상태로 돌려놓고

        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)                         //현재 열려있는 모든 인벤토리의 슬롯을 확인하며
        {
            for (int j = 0; j < InventoryManager.instance.openInventoryList[i].slots.GetLength(1); j++)
            {
                for (int k = 0; k < InventoryManager.instance.openInventoryList[i].slots.GetLength(0); k++)
                {
                    if (InventoryManager.instance.openInventoryList[i].slots[k, j].assignedItem != null)    //그 슬롯에 아이템이 있지 않다면
                    {
                        if (InventoryManager.instance.openInventoryList[i].slots[k, j].assignedItem.TryGetComponent<ExpendableItem>(out ExpendableItem eItem) == true) //그 아이템이 나눌 수 있는 아이템이라면
                        {
                            if (eItem.ReadyCombineItem == true)                     //그 나눌 수 있는 아이템을 합쳐야 한다면
                            {
                                eItem.CombineItem(GetComponent<ExpendableItem>());  //두 아이템을 합쳐라
                            }
                        }
                    }

                    if (InventoryManager.instance.openInventoryList[i].slots[k, j].IsSelected)          //슬롯 중에서 현재 아이템을 넣어 줄 준비가 된 슬롯이라면
                    {
                        InventoryManager.instance.openInventoryList[i].slots[k, j].assignedItem = this; //그 슬롯에 이 아이템을 넣고
                        InventoryManager.instance.openInventoryList[i].slots[k, j].SetPosition(this);   //위치를 잡아준다(아이템의 위치를 잡기 위해 왼쪽 밑의 위치를 찾음)
                        assignedSlot.Add(InventoryManager.instance.openInventoryList[i].slots[k, j]);   //현재 지정된 슬롯 리스트에 이 슬롯들을 넣음
                        canSetPosition = true;
                    }
                    InventoryManager.instance.openInventoryList[i].slots[k, j].ResetColor();            //슬롯들의 색깔을 초기화
                }
            }
        }

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].SetWeapon(this))
            {
                Destroy(gameObject);
                canSetPosition = true;
            }

        }

        if (canSetPosition == false)                //포지션을 설정할 수 없는 상황이라면
        {
            GoToLastPosition();                      //지난번 위치로 가라
        }
        else
        {
            lastSlot.Clear();
            for (int i = 0; i < assignedSlot.Count; i++)
            {
                lastSlot.Add(assignedSlot[i]);       //지난 슬롯 리스트에도 이 슬롯들을 넣음
            }
        }

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;



    }




    private void GoToLastPosition()
    {
        for (int i = 0; i < lastSlot.Count; i++) //모든 지난 슬롯의 지정된 아이템은 이것이다
        {
            lastSlot[i].assignedItem = this;
            assignedSlot.Add(lastSlot[i]);
        }

        int r = (int)Mathf.Abs(rectTransform.eulerAngles.z - lastRotation) / 10;

        if (r * 10 % 180 != 0)
        {
            int temp = sizeX;
            sizeX = sizeY;
            sizeY = temp;
        }

        rectTransform.eulerAngles = new Vector3(0, 0, lastRotation);
        SetPosition(lastPosition);              //지난 위치로 돌아감
    }



    public void Setup()
    {
        InventoryManager.instance.currentItem = null;       //인벤토리 메니저에 현재 아이템을 들고있지 않다고 해줌

        image.raycastTarget = true;                         //레이케스트 타겟을 켜줌

        Color c = image.color;                              //색깔, 크기, 배경 원래대로
        c.a = 1f;
        image.color = c;

        background.gameObject.SetActive(false);

        transform.localScale = new Vector3(1, 1, 1);
        isItemSelected = false;
    }



    private void CalculateClickedPoint()    //클릭한 위치랑 아이템의 중심의 오차 범위를 구함
    {
        int errorRangeX;
        int errorRangeY;

        Vector2 c = clickPoint;

        if (sizeX % 2 != 0)
        {
            if (c.x < 0) c.x -= 50;
            else c.x += 50;
        }


        if (sizeY % 2 != 0)
        {
            if (c.x < 0) c.y -= 50;
            else c.y += 50;
        }

        if (c.x < 0) errorRangeX = ((int)c.x - 100) / 100;
        else errorRangeX = ((int)c.x + 100) / 100;

        if (c.y < 0) errorRangeY = ((int)c.y - 100) / 100;
        else errorRangeY = ((int)c.y + 100) / 100;


        if (sizeX % 2 != 0)
        {
            if (errorRangeX > 0) errorRangeX--;
            else errorRangeX++;

            if (Mathf.Abs(c.x) < 100)
            {
                errorRangeX = 0;
            }
        }

        if (sizeY % 2 != 0)
        {
            if (errorRangeY > 0) errorRangeY--;
            else errorRangeY++;

            if (Mathf.Abs(c.y) < 100)
            {
                errorRangeY = 0;
            }
        }

        clickPointInt = new Vector2Int(errorRangeX, errorRangeY);
    }

    public void RotateThis()
    {
        Vector3 rotation = new Vector3(0, 0, rectTransform.eulerAngles.z + 90);

        rectTransform.eulerAngles = rotation;

        int temp = sizeX;
        sizeX = sizeY;
        sizeY = temp;
    }

    protected virtual void RotateItem()
    {
        if (Input.GetKeyDown(KeyCode.R) && isItemSelected == true)   //R키를 눌렀을 때 이미 회전 중이 아니고 지금 들고 있는 아이템이라면
        {
            UISoundManager.instance.PlayOneShot(Sound.ButtonClilckSound);

            StartCoroutine(Rotate());
        }
    }

    IEnumerator Rotate()
    {
        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)
        {
            for (int j = 0; j < InventoryManager.instance.openInventoryList[i].Height; j++)
            {
                for (int k = 0; k < InventoryManager.instance.openInventoryList[i].Width; k++)
                {
                    InventoryManager.instance.openInventoryList[i].slots[k, j].ResetColor();
                }
            }
        }

        Vector3 rotation = new Vector3(0, 0, rectTransform.eulerAngles.z + 90);

        rectTransform.eulerAngles = rotation;

        float temp1 = clickPoint.x;
        clickPoint.x = clickPoint.y;
        clickPoint.y = temp1;

        int temp = sizeX;
        sizeX = sizeY;
        sizeY = temp;

        image.raycastTarget = true;
        yield return null;

        image.raycastTarget = false;
    }


    public void SetPosition(Vector2 position)
    {
        image.raycastTarget = true;
        rectTransform.anchoredPosition = position;
        lastPosition = position;
        lastRotation = rectTransform.eulerAngles.z;
    }

    public void SetOnlyPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public void Init(Normal normalItem, float rotation)
    {
        normalItemStruct = normalItem;
        itemType = normalItem.itemType;
        Vector3 rot = new Vector3(0, 0, rotation);

        rectTransform.eulerAngles = rot;
        if ((int)rotation % 180 != 0)
        {
            int temp = normalItem.size.x;
            normalItem.size.x = normalItem.size.y;
            normalItem.size.y = temp;
        }

        sizeX = normalItem.size.x;
        sizeY = normalItem.size.y;

        lastRotation = rectTransform.eulerAngles.z;
        lastPosition = rectTransform.anchoredPosition;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true) return;

        UISoundManager.instance.PlayOneShot(Sound.CursurMoveSound);
    }
}
