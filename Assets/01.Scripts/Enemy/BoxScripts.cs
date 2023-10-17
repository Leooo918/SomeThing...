using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(OpenInventory))]
public class BoxScripts : MonoBehaviour
{
    public EnemyBoxSO boxSO = null;
    public ItemSO itemSO = null;
    private OpenInventory openInventory = null;
    private OpenInventory playerBackPack = null;
    private Transform itemParent = null;
    private RectTransform obstacles = null;
    private Button btn = null;
    private PlayerInput input = null;

    private float routingTime = 2.5f;

    private void Start()
    {
        playerBackPack = GameManager.instance.player.GetComponent<OpenInventory>();
        openInventory = GetComponent<OpenInventory>();
        itemParent = GameManager.instance.canvas;
        obstacles = itemParent.Find("Obstacle").GetComponent<RectTransform>();
        btn = obstacles.Find("NotFound").GetComponent<Button>();
        obstacles.gameObject.SetActive(false);

        Init("Dog");                            //일단 디버그용 나중에 애너미 죽을때 애너미 이름으로 Init해주게 봐꺼야 함
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

        RectTransform r = openInventory.myInventory.transform.Find("InventoryBackground").GetComponent<RectTransform>();
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
        openInventory.InventoryClose();
        playerBackPack.InventoryClose();
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

    public void Init(string monsterName)
    {
        openInventory.Init();

        EnemyBox e = new EnemyBox();
        itemParent = GameManager.instance.canvas;

        for (int i = 0; i < boxSO.enemyBoxes.Length; i++)
        {
            if (boxSO.enemyBoxes[i].enemyName == monsterName)
            {
                e = boxSO.enemyBoxes[i];

                for (int j = 0; j < e.items.Length; j++)     //이 적에게서 나올수 있는 아이템들을 모두 돌며
                {
                    if (Random.Range(0, 100) < e.items[j].exisistPercentage)  //그 아이템이 나올 확률을 계산해
                    {
                        int itemNum = Random.Range(1, e.items[j].maxItemAmount);

                        print(itemNum);
                        Item it;
                        ExpendableItem ex;

                        for (int k = 0; k < itemSO.items.Count; k++)
                        {
                            if (itemSO.items[k].itemName == e.items[j].itemName)
                            {
                                for (int l = 0; l < itemNum; l++)
                                {
                                    GameObject g = Instantiate(itemSO.items[k].pfItem, itemParent);
                                    g.transform.SetAsLastSibling();
                                    g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

                                    it = g.GetComponent<Item>();

                                    print(it);
                                    it.Init(itemSO.items[k], 0);
                                    openInventory.myInventory.SetItem(it);
                                }
                                break;
                            }
                        }
                        for (int k = 0; k < itemSO.expendableItems.Count; k++)
                        {
                            if (itemSO.expendableItems[k].itemName == e.items[j].itemName)
                            {
                                while (itemNum > 0)
                                {
                                    GameObject g = Instantiate(itemSO.expendableItems[k].pfItem, itemParent);
                                    g.transform.SetAsLastSibling();
                                    g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

                                    ex = g.GetComponent<ExpendableItem>();

                                    int num = Mathf.Clamp(itemNum, 1, ex.MaxitemNum);

                                    ex.Init(itemSO.expendableItems[k], num, 0);
                                    openInventory.myInventory.SetItem(ex);
                                    itemNum -= num;
                                }
                                break;
                            }
                        }

                    }
                }

            }
        }
    }
}
