using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    private RectTransform interactUI = null;
    private RectTransform unEquipUI = null;
    private RectTransform playerStatusUI = null;
    private RectTransform itemBuyHelper = null;
    private RectTransform itemBuyError = null;
    private RectTransform itemDeviderHelper = null;
    private RectTransform obstacles = null;
    private List<OpenInventory> inventories = new List<OpenInventory>();
    private Inventory enemyBoxInventory = null;



    public RectTransform UnEquipUI => unEquipUI;
    public Inventory EnemyBoxInventory => enemyBoxInventory;

    public void OnInteract(bool isInteract)
    {
        if (isInteract == true)  //상호작용 
        {
            interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(1, 0.3f).SetEase(Ease.Linear);
        }
        else                    //상호작용 해제한 거냐
        {

            interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(0, 0.3f).SetEase(Ease.Linear);
        }
    }

    public void UnEquip(Vector2 position, UnityAction call)
    {
        unEquipUI.gameObject.SetActive(true);
        unEquipUI.anchoredPosition = position;
        Button b = unEquipUI.transform.Find("Button").GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(call);
    }

    public void SetHp(float maxHp, float currentHp)
    {
        playerStatusUI.Find("PlayerStatus/Hp/Sprite").GetComponent<Image>().fillAmount = currentHp / maxHp;
        playerStatusUI.Find("PlayerStatus/Hp/Value").GetComponent<TextMeshProUGUI>().SetText($"{currentHp}<sub><size=55>/{maxHp}");
    }

    public void Init(Transform canvas, PlayerStatus player, ItemSO itemSO, WeaponSO weaponSO)
    {
        interactUI = canvas.Find("InteractionUI").GetComponent<RectTransform>();
        unEquipUI = canvas.Find("Unequip").GetComponent<RectTransform>();
        playerStatusUI = canvas.Find("PlayerStatusBackground").GetComponent<RectTransform>();
        itemBuyHelper = canvas.Find("BuyHelper").GetComponent<RectTransform>();
        itemBuyError = canvas.Find("CannotBuyItem").GetComponent<RectTransform>();
        itemDeviderHelper = canvas.Find("ItemDeviderHelper").GetComponent<RectTransform>();
        obstacles = canvas.Find("Obstacle").GetComponent<RectTransform>();

        obstacles.gameObject.SetActive(false);

        inventories = FindObjectsByType<OpenInventory>(FindObjectsSortMode.InstanceID).ToList();

        PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[3];
        for (int i = 0; i < 3; i++)
        {
            weaponSlots[i] = playerStatusUI.Find("PlayerStatue/MountingWeapons").GetChild(i).GetComponent<PlayerWeaponSlot>();
        }

        player.Init(itemSO, weaponSO, playerStatusUI.gameObject, weaponSlots);
    }
}