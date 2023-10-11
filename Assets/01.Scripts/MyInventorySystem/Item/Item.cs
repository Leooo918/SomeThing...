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
    public string itemName = "";                            //������ �̸�

    protected ItemType itemType = new ItemType();
    private Vector2 clickPoint = Vector2.zero;              //Ŭ���� ��ġ
    private Vector2Int clickPointInt = Vector2Int.zero;     //Ŭ���� ��ġ�� �������� ��ġ ����

    protected int sizeX, sizeY;                             //�������� ������
    protected Normal normalItemStruct;


    protected RectTransform rectTransform = null;           //������ RectTransform
    protected Image image = null;                             //������ �̹���
    protected Image[] backgroundImage;                        //Ŭ������ �� ������ ���
    protected Transform background = null;                    //��� ���ӿ�����Ʈ 
    protected Camera mainCamera = null;                       //���� ī�޶�

    //ȸ���� ���� ����
    protected bool isItemSelected = false;                    //������ ȸ���� �� ��� �ִ� �����۸� ȸ����Ű�� ���� ��

    //�������� ���� ��ġ�� ������ �����ϴ� ��
    public Vector2 lastPosition = Vector2.zero;             //�������� ���� ��ġ
    public List<Slot> lastSlot = new List<Slot>();          //�������� ������ִ� ��� ����
    public float lastRotation = 0;


    public List<Slot> assignedSlot = new List<Slot>();      //���� �������� ���Ե��ִ� ��� ����
    private PlayerWeaponSlot[] weaponSlots = null;

    #region ������Ƽ
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
        CalculateClickedPoint();    //Ŭ���� ��ġ�� �������� ��ġ�� ������ ���� (ex) x��ǥ�� -1ĭ, y��ǥ�� -1ĭ) clickPointInt�� �����
        RotateItem();               //������ ȸ��
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;
        transform.Find("Sprite").GetComponent<Image>().color = new Color(1, 1, 1, 1);

        UISoundManager.instance.PlayOneShot(Sound.ButtonClilckSound);         //�Ҹ��� ��򳪰� ������

        image.raycastTarget = false;

        for (int i = 0; i < assignedSlot.Count; i++)                        //���� ����Ǿ� �ִ� ��� ������ �ʱ�ȭ
        {
            assignedSlot[i].assignedItem = null;
        }
        assignedSlot.Clear();

        InventoryManager.instance.currentItem = this;                       //�κ��丮 �޴����� ���� �� �������� ��� �ִٰ� ����

        clickPoint = eventData.position - rectTransform.anchoredPosition;   //Ŭ���� ��ġ�� �� �������� ���������ؼ� �޾ƿ�

        //���õǸ� �������� ��¦ ũ���ϰ� �ڿ� ����� ���̰� �ϰ� ���İ��� �����
        Color c = image.color;
        c.a = 0.7f;
        image.color = c;
        background.gameObject.SetActive(true);
        transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

        isItemSelected = true;                                              //�� �������� ���� ���õǾ���
    }



    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;
        //�巡�� �� �� ���콺 ���� �����̰�
        rectTransform.anchoredPosition = eventData.position + new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize) - clickPoint;
    }


    public virtual void OnPointerUp(PointerEventData eventData)
    {

        if (eventData.button != PointerEventData.InputButton.Left || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;

        UISoundManager.instance.PlayOneShot(Sound.ButtonClilckSound);

        bool canSetPosition = false;
        Setup();                        //������ �ʱ� ���·� ��������

        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)                         //���� �����ִ� ��� �κ��丮�� ������ Ȯ���ϸ�
        {
            for (int j = 0; j < InventoryManager.instance.openInventoryList[i].slots.GetLength(1); j++)
            {
                for (int k = 0; k < InventoryManager.instance.openInventoryList[i].slots.GetLength(0); k++)
                {
                    if (InventoryManager.instance.openInventoryList[i].slots[k, j].assignedItem != null)    //�� ���Կ� �������� ���� �ʴٸ�
                    {
                        if (InventoryManager.instance.openInventoryList[i].slots[k, j].assignedItem.TryGetComponent<ExpendableItem>(out ExpendableItem eItem) == true) //�� �������� ���� �� �ִ� �������̶��
                        {
                            if (eItem.ReadyCombineItem == true)                     //�� ���� �� �ִ� �������� ���ľ� �Ѵٸ�
                            {
                                eItem.CombineItem(GetComponent<ExpendableItem>());  //�� �������� ���Ķ�
                            }
                        }
                    }

                    if (InventoryManager.instance.openInventoryList[i].slots[k, j].IsSelected)          //���� �߿��� ���� �������� �־� �� �غ� �� �����̶��
                    {
                        InventoryManager.instance.openInventoryList[i].slots[k, j].assignedItem = this; //�� ���Կ� �� �������� �ְ�
                        InventoryManager.instance.openInventoryList[i].slots[k, j].SetPosition(this);   //��ġ�� ����ش�(�������� ��ġ�� ��� ���� ���� ���� ��ġ�� ã��)
                        assignedSlot.Add(InventoryManager.instance.openInventoryList[i].slots[k, j]);   //���� ������ ���� ����Ʈ�� �� ���Ե��� ����
                        canSetPosition = true;
                    }
                    InventoryManager.instance.openInventoryList[i].slots[k, j].ResetColor();            //���Ե��� ������ �ʱ�ȭ
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

        if (canSetPosition == false)                //�������� ������ �� ���� ��Ȳ�̶��
        {
            GoToLastPosition();                      //������ ��ġ�� ����
        }
        else
        {
            lastSlot.Clear();
            for (int i = 0; i < assignedSlot.Count; i++)
            {
                lastSlot.Add(assignedSlot[i]);       //���� ���� ����Ʈ���� �� ���Ե��� ����
            }
        }

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || InventoryManager.instance.isDeviding == true || ShopManager.instance.isBuying == true) return;



    }




    private void GoToLastPosition()
    {
        for (int i = 0; i < lastSlot.Count; i++) //��� ���� ������ ������ �������� �̰��̴�
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
        SetPosition(lastPosition);              //���� ��ġ�� ���ư�
    }



    public void Setup()
    {
        InventoryManager.instance.currentItem = null;       //�κ��丮 �޴����� ���� �������� ������� �ʴٰ� ����

        image.raycastTarget = true;                         //�����ɽ�Ʈ Ÿ���� ����

        Color c = image.color;                              //����, ũ��, ��� �������
        c.a = 1f;
        image.color = c;

        background.gameObject.SetActive(false);

        transform.localScale = new Vector3(1, 1, 1);
        isItemSelected = false;
    }



    private void CalculateClickedPoint()    //Ŭ���� ��ġ�� �������� �߽��� ���� ������ ����
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
        if (Input.GetKeyDown(KeyCode.R) && isItemSelected == true)   //RŰ�� ������ �� �̹� ȸ�� ���� �ƴϰ� ���� ��� �ִ� �������̶��
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
