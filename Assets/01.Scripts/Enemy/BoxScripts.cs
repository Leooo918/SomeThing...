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
    private PlayerInput input = null;
    private Transform interactUI;

    private void Start()
    {
        playerBackPack = GameManager.instance.player.GetComponent<OpenInventory>();
        openInventory = GetComponent<OpenInventory>();
        itemParent = GameManager.instance.canvas;
        interactUI = itemParent.Find("InteractionUI");

        Init("Dog");

        openInventory.InventoryClose();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInput>(out input))
        {
            input.onInteraction += OnInteraction;
            interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(1, 0.3f).SetEase(Ease.Linear);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerInput>(out input))
        {
            input.onInteraction -= OnInteraction;
            interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(0, 0.3f).SetEase(Ease.Linear);
            OnExit();
        }
    }

    private void OnInteraction()
    {
        openInventory.InventoryOpen();
        playerBackPack.InventoryOpen();
        interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(0, 0.3f).SetEase(Ease.Linear);
    }

    private void OnExit()
    {
        openInventory.InventoryClose();
        playerBackPack.InventoryClose();
    }

    public void Init(string monsterName)
    {
        EnemyBox e = new EnemyBox();
        itemParent = GameManager.instance.canvas;

        for (int i = 0; i < boxSO.enemyBoxes.Length; i++)
        {
            if (boxSO.enemyBoxes[i].enemyName == monsterName)
            {
                e = boxSO.enemyBoxes[i];
                openInventory.InventoryOpen();

                for (int j = 0; j < e.items.Length; j++)     //이 적에게서 나올수 있는 아이템들을 모두 돌며
                {
                    if (Random.Range(0, 100) < e.items[j].exisistPercentage)  //그 아이템이 나올 확률을 계산해
                    {
                        Item it;

                        for (int k = 0; k < itemSO.items.Count; k++)
                        {
                            if (itemSO.items[k].itemName == e.items[j].itemName)
                            {
                                GameObject g = Instantiate(itemSO.items[k].pfItem, itemParent);
                                g.transform.SetAsLastSibling();
                                g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

                                it = g.GetComponent<Item>();
                                it.Init(itemSO.items[k], 0);
                                openInventory.myInventory.SetItem(it);
                                break;
                            }
                        }
                        for (int k = 0; k < itemSO.expendableItems.Count; k++)
                        {
                            if (itemSO.expendableItems[k].itemName == e.items[j].itemName)
                            {
                                GameObject g = Instantiate(itemSO.expendableItems[k].pfItem, itemParent);
                                g.transform.SetAsLastSibling();
                                g.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

                                it = g.GetComponent<Item>();
                                it.Init(itemSO.items[k], 0);
                                openInventory.myInventory.SetItem(it);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
}
