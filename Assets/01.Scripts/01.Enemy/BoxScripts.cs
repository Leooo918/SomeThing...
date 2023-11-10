using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(OpenInventory))]
public class BoxScripts : MonoBehaviour
{
    private string boxMonsterName = "";

    [SerializeField]private EnemyBoxSO boxSO = null;        //임시
    private ItemSO itemSO = null;
    private Renderer render = null;
    private OpenInventory openInventory = null;
    private OpenInventory playerBackPack = null;
    private Transform itemParent = null;
    private RectTransform obstacles = null;
    private Button btn = null;
    private PlayerInput input = null;
    private Button closeBtn = null;
    private Transform warning = null;
    private Button cancel = null;
    private Button accept = null;
    private Toggle checkWarning = null;

    private float routingTime = 2.5f;


    private void Awake()
    {
        render = GetComponent<Renderer>();
        playerBackPack = GameManager.instance.player.GetComponent<OpenInventory>();
        openInventory = GetComponent<OpenInventory>();
        itemParent = GameManager.instance.canvas;
        obstacles = itemParent.Find("Obstacle").GetComponent<RectTransform>();
        btn = obstacles.Find("NotFound").GetComponent<Button>();
        obstacles.gameObject.SetActive(false);

        openInventory.onOpenInventory += OnOpenInventory;
    }

    private void OnDisable()
    {
        StopCoroutine("DisolveRoutine");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInput>(out input))
        {
            input.onInteraction += OnInteraction;
            UIManager.instance.OnInteract(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInput>(out input))
        {
            input.onInteraction -= OnInteraction;
            UIManager.instance.OnInteract(false);
            OnExit();
        }
    }

    private void OnInteraction()
    {
        openInventory.InventoryOpen();
        playerBackPack.InventoryOpen();
        btn.gameObject.SetActive(true);
        obstacles.gameObject.SetActive(true);
        obstacles.SetAsLastSibling();

        RectTransform r = openInventory.MyInventory.transform.Find("InventoryBackground").GetComponent<RectTransform>();
        obstacles.anchoredPosition = r.anchoredPosition;
        obstacles.sizeDelta = r.sizeDelta;

        btn.onClick.AddListener(() =>
        {
            btn.gameObject.SetActive(false);
            StartCoroutine("ItemSerchRoutine");
        });
        UIManager.instance.OnInteract(false);
    }

    private void OnExit()
    {
        if (openInventory.MyInventory.gameObject.activeSelf == true)
        {
            openInventory.InventoryClose();
            playerBackPack.InventoryClose();
            warning.gameObject.SetActive(false);
            StopCoroutine("LoadingCircle");
            obstacles.gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine("DisolveRoutine");
        }
    }

    IEnumerator DisolveRoutine()
    {
        float a = 0;
        while (render.material.GetFloat("_DisolveValue") > 0)
        {
            a += Time.deltaTime / 1.2f;
            render.material.SetFloat("_DisolveValue", Mathf.Lerp(1, 0, a));
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator ItemSerchRoutine()
    {
        StopCoroutine("LoadingCircle");
        StartCoroutine("LoadingCircle");
        yield return new WaitForSeconds(routingTime);
        StopCoroutine("LoadingCircle");
        obstacles.gameObject.SetActive(false);
    }

    IEnumerator LoadingCircle()
    {
        RectTransform r = obstacles.transform.Find("LoadingCircle").GetComponent<RectTransform>();
        while (true)
        {
            float rot = r.eulerAngles.z + 1.5f;
            if (rot > 360)
            {
                rot = 0;
            }
            r.eulerAngles = new Vector3(0, 0, rot);
            yield return null;
        }
    }

    private void SetWarning()
    {
        closeBtn = openInventory.MyInventory.transform.Find("CloseBtn").GetComponent<Button>();
        warning = UIManager.instance.WarningBoxExit;
        cancel = warning.Find("Warning/NoBtn").GetComponent<Button>();
        accept = warning.Find("Warning/OkBtn").GetComponent<Button>();
        checkWarning = warning.transform.Find("CheckWarning").GetComponent<Toggle>();

        warning.gameObject.SetActive(false);

        checkWarning.onValueChanged.RemoveAllListeners();
        checkWarning.onValueChanged.AddListener(e =>
        {
            SettingManager.instance.SaveCheckWarning(e);
        });

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() =>
        {
            if (SettingManager.instance.LoadCheckWarning() == true)
            {
                OnExit();
            }
            else
            {
                warning.gameObject.SetActive(true);
                warning.SetAsLastSibling();
            }
        });

        accept.onClick.RemoveAllListeners();
        accept.onClick.AddListener(OnExit);
        cancel.onClick.AddListener(() =>
        {
            warning.gameObject.SetActive(false);
        });
    }

    private void OnOpenInventory()
    {
        openInventory.Init();
        SetWarning();

        
        itemParent = GameManager.instance.canvas;

        for (int j = 0; j < boxSO.items.Length; j++)     //이 적에게서 나올수 있는 아이템들을 모두 돌며
        {
            if (Random.Range(0, 100) < boxSO.items[j].exisistPercentage)  //그 아이템이 나올 확률을 계산해
            {
                int itemNum = Random.Range(1, boxSO.items[j].maxItemAmount+1);

                Item it;
                ExpendableItem ex;

                for (int k = 0; k < itemSO.items.Count; k++)
                {
                    if (itemSO.items[k].itemName == boxSO.items[j].itemName)
                    {
                        for (int l = 0; l < itemNum; l++)
                        {
                            GameObject g = Instantiate(itemSO.items[k].pfItem, itemParent);
                            g.transform.SetAsLastSibling();
                            g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

                            it = g.GetComponent<Item>();
                            g.transform.localScale = new Vector3(2, 2, 2);

                            it.Init(itemSO.items[k], 0);
                            if (it.TryGetComponent<ItemWeapon>(out ItemWeapon w))
                            {
                                w.Init(100);
                            }

                            openInventory.MyInventory.SetItem(it);
                            it.gameObject.SetActive(false);
                        }
                        break;
                    }
                }
                for (int k = 0; k < itemSO.expendableItems.Count; k++)
                {
                    if (itemSO.expendableItems[k].itemName == boxSO.items[j].itemName)
                    {
                        GameObject g = Instantiate(itemSO.expendableItems[k].pfItem, itemParent);
                        g.transform.SetAsLastSibling();
                        g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                        g.transform.localScale = new Vector3(2, 2, 2);
                        ex = g.GetComponent<ExpendableItem>();
                        while (itemNum > 0)
                        {
                            int num = Mathf.Clamp(itemNum, 1, itemSO.expendableItems[k].maxItemNum);

                            ex.Init(itemSO.expendableItems[k], num, 0);
                            openInventory.MyInventory.SetItem(ex);
                            itemNum -= num;
                            ex.gameObject.SetActive(false);
                        }
                        break;
                    }
                }
            }
        }


        openInventory.InventoryClose();
    }

    public void Init(EnemyBoxSO boxSO)
    {
        this.boxSO = boxSO;

        itemSO = GameManager.instance.itemSO;
    }
}
