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
    private RectTransform warningBoxExit = null;
    private PlayerLanternSlot lanternSlot = null;
    private Transform checkProfileUI = null;
    private List<OpenInventory> inventories = new List<OpenInventory>();
    private Inventory enemyBoxInventory = null;
 

    public PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[3];
    public RectTransform UnEquipUI => unEquipUI;
    public Inventory EnemyBoxInventory => enemyBoxInventory;
    public RectTransform WarningBoxExit => warningBoxExit;
    public PlayerLanternSlot LanternSlot => lanternSlot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetProfile(new Vector3(1,1,1), "Katana", "이 아이템은 영국으로부터 시작하여 머시깽이 머시깽이 머시깽이 머시깽이 ㅇㅋ? 유남쌩? 대충 이러쿵 저러쿵 해서 존나 개 쩌는 카타나인데 이걸 대충 주인공이 가지고 있다 그런 내용이다 와 개쩐다 ㄹㅇㅋㅋ", true, 89.5f, 2, 40.6f);
        }
    }

    public void OnInteract(bool isInteract)
    {
        if (isInteract == true)  //상호작용 
        {
            interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(1, 0.3f).SetEase(Ease.Linear).SetAutoKill(true);
        }
        else                    //상호작용 해제한 거냐
        {
            interactUI.Find("Text").GetComponent<TextMeshProUGUI>().DOFade(0, 0.3f).SetEase(Ease.Linear).SetAutoKill(true);
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

    public void SetProfile(Vector3 position, string itemName, string itemExplain, bool isWeapon = false, float durability = 0, int proficiencyLv = 0, float proficiencyProgress = 0)
    {
        checkProfileUI.gameObject.SetActive(true);
        checkProfileUI.GetComponent<RectTransform>().anchoredPosition = position;
        checkProfileUI.Find("Background/Btn").GetComponent<Button>().onClick.AddListener(() =>
        {
            checkProfileUI.gameObject.SetActive(false); 
        });

        TextMeshProUGUI itemNameText = checkProfileUI.Find("Background/ItemName").GetComponent<TextMeshProUGUI>();
        itemNameText.SetText(itemName);

        Transform durabilityObj = checkProfileUI.Find("Background/Durability");
        Transform proficiencyObj = checkProfileUI.Find("Background/Proficiency");

        RectTransform contents = checkProfileUI.Find("Background/Explain/Text/Viewport/Content").GetComponent<RectTransform>();
        TextMeshProUGUI explainTxt = contents.Find("TextSize/Text").GetComponent<TextMeshProUGUI>();

        explainTxt.SetText(itemExplain);
        contents.sizeDelta = new Vector3(contents.sizeDelta.x, explainTxt.GetComponent<RectTransform>().rect.height, 1);

        if (isWeapon == false)
        {
            durabilityObj.gameObject.SetActive(false);
            proficiencyObj.gameObject.SetActive(false);
        }
        else
        {
            durabilityObj.gameObject.SetActive(true);
            proficiencyObj.gameObject.SetActive(true);

            durabilityObj.Find("Value").GetComponent<TextMeshProUGUI>().SetText($"{durability}/100");
            proficiencyObj.Find("Value").GetComponent<TextMeshProUGUI>().SetText($"Lv.{proficiencyLv}");
            proficiencyObj.Find("Progress/Value").GetComponent<Image>().fillAmount = proficiencyProgress / 100;
        }
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
        warningBoxExit = canvas.Find("Warning").GetComponent<RectTransform>();
        checkProfileUI = canvas.Find("CheckProfile");

        obstacles.gameObject.SetActive(false);
        warningBoxExit.gameObject.SetActive(false);
        checkProfileUI.gameObject.SetActive(false);
        lanternSlot = playerStatusUI.Find("PlayerStatue/PlayerLanternSlot").GetComponent<PlayerLanternSlot>();

        inventories = FindObjectsByType<OpenInventory>(FindObjectsSortMode.InstanceID).ToList();

        for (int i = 0; i < 3; i++)
        {
            weaponSlots[i] = playerStatusUI.Find("PlayerStatue/MountingWeapons").GetChild(i).GetComponent<PlayerWeaponSlot>();
        }

        player.Init(itemSO, weaponSO, playerStatusUI.gameObject, weaponSlots, lanternSlot);
    }
}