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
    private Transform skillCheck = null;
    private Transform playerSkills = null;
    private Image mainSkillCool = null;
    private Image subSkillCool = null;



    public PlayerWeaponSlot[] weaponSlots = new PlayerWeaponSlot[2];
    public RectTransform UnEquipUI => unEquipUI;
    public Inventory EnemyBoxInventory => enemyBoxInventory;
    public RectTransform WarningBoxExit => warningBoxExit;
    public PlayerLanternSlot LanternSlot => lanternSlot;
    public Transform SkillCheck => skillCheck;
    public Image MainSkillCool => mainSkillCool;
    public Image SubSkillCool => subSkillCool;
    public Transform PlayerSkills => playerSkills;

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
        checkProfileUI.SetAsLastSibling();
        checkProfileUI.gameObject.SetActive(true);

        RectTransform r = checkProfileUI.GetComponent<RectTransform>();

        position = new Vector3(Mathf.Clamp(position.x, 0, 1920 - r.rect.width), Mathf.Clamp(position.y, r.rect.height / 2, 1080 - r.rect.height / 2), 0);
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
            print(proficiencyProgress);
        }
    }

    public void SetPlayerSkill(float durability, GameObject weaponImage, GameObject mainSkill, GameObject subSkill)
    {
        print(weaponImage.name);
        playerSkills.Find("Weapon/Frame/Durability").GetComponent<Image>().fillAmount = durability / 100f;

        if (playerSkills.Find("Weapon/Frame").childCount > 1)
        {
            for (int i = 1; i < playerSkills.Find("Weapon/Frame").childCount; i++)
            {
                Destroy(playerSkills.Find("Weapon/Frame").GetChild(i).gameObject);
            }
        }
        if (playerSkills.Find("MainSkillFrame").childCount > 0)
        {
            for (int i = 0; i < playerSkills.Find("MainSkillFrame").childCount; i++)
            {
                Destroy(playerSkills.Find("MainSkillFrame").GetChild(i).gameObject);
            }
        }
        if (playerSkills.Find("SubSkillFrame").childCount > 0)
        {
            for (int i = 0; i < playerSkills.Find("SubSkillFrame").childCount; i++)
            {
                Destroy(playerSkills.Find("SubSkillFrame").GetChild(i).gameObject);
            }
        }

        RectTransform weaponimage = Instantiate(weaponImage, playerSkills.Find("Weapon/Frame")).GetComponent<RectTransform>();
        weaponimage.anchoredPosition3D = new Vector3(0, 0, 0);
        weaponimage.localScale = new Vector3(1, 1, 1);

        RectTransform mainSkillImag = Instantiate(mainSkill, playerSkills.Find("MainSkillFrame")).GetComponent<RectTransform>();
        mainSkillImag.anchoredPosition3D = new Vector3(0, 0, 0);
        mainSkillImag.localScale = new Vector3(1, 1, 1);

        RectTransform subSkillImg = Instantiate(subSkill, playerSkills.Find("SubSkillFrame")).GetComponent<RectTransform>();
        subSkillImg.anchoredPosition3D = new Vector3(0, 0, 0);
        subSkillImg.localScale = new Vector3(1, 1, 1);

        mainSkillCool = mainSkillImag.Find("SkillIcon/SkillCoolDown").GetComponent<Image>();
        subSkillCool = subSkillImg.Find("SkillIcon/SkillCoolDown").GetComponent<Image>();
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
        lanternSlot = playerStatusUI.Find("PlayerStatue/PlayerLanternSlot").GetComponent<PlayerLanternSlot>();
        inventories = FindObjectsByType<OpenInventory>(FindObjectsSortMode.InstanceID).ToList();
        skillCheck = canvas.Find("SkillCheck");
        playerSkills = canvas.Find("PlayerSkill");

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i] = playerStatusUI.Find("PlayerStatue/MountingWeapons").GetChild(i).GetComponent<PlayerWeaponSlot>();
        }
        player.Init(itemSO, weaponSO, playerStatusUI.gameObject, weaponSlots, lanternSlot);

        skillCheck.gameObject.SetActive(false);
        obstacles.gameObject.SetActive(false);
        warningBoxExit.gameObject.SetActive(false);
        checkProfileUI.gameObject.SetActive(false);


    }
}