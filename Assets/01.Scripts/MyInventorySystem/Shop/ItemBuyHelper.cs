using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ItemBuyHelper : MonoBehaviour
{
    private Button cancelBtn = null;
    private Button buyBtn = null;
    private Slider slider = null;
    private TMP_InputField inputField = null;
    private Image image = null;
    private Button valueUpBtn = null;
    private Button valueDownBtn = null;
    private TextMeshProUGUI valueTxt = null;
    private TextMeshProUGUI maxAmountTxt = null;
    private GameObject errorScreen = null;

    private ShopGoods goodsToBuy = null;
    private int amount = 10;

    public string moneylackless = "돈이 조금 부족 합니다!";
    public string spacelackless = "창고에 공간이 부족 합니다!";

    private void Awake()
    {
        buyBtn = transform.Find("BuyBtn").GetComponent<Button>();
        cancelBtn = transform.Find("CancelBtn").GetComponent<Button>();

        image = transform.Find("Image").GetComponent<Image>();

        slider = transform.Find("Slider").GetComponent<Slider>();
        valueUpBtn = slider.transform.Find("Add").GetComponent<Button>();
        valueDownBtn = slider.transform.Find("Minus").GetComponent<Button>();
        inputField = slider.transform.Find("InputField").GetComponent<TMP_InputField>();

        valueTxt = transform.Find("ValueTxt").GetComponent<TextMeshProUGUI>();
        maxAmountTxt = slider.transform.Find("InputField/MaxText").GetComponent<TextMeshProUGUI>();
    }


    private void OnValueChange(string value)
    {
        int devideNum = 0;

        if (int.TryParse(value, out int i) == true) devideNum = Mathf.Clamp(i, 0, amount);
        else devideNum = 1;

        inputField.text = devideNum.ToString();
        slider.value = devideNum;

        valueTxt.SetText((goodsToBuy.Value * amount).ToString());
    }

    private void OnSliderValueChange(float value)
    {
        slider.value = (int)value;
        inputField.text = slider.value.ToString();

        valueTxt.SetText((goodsToBuy.Value * amount).ToString());
    }

    private void EndShopping()
    {
        ShopManager.instance.isBuying = false;
        gameObject.SetActive(false);
    }

    private void BuyItem(ShopGoods goodsToBuy)
    {
        if ((int)slider.value <= 0)
        {
            EndShopping();
            return;
        }

        if ((int)slider.value * goodsToBuy.Value > 50) //나중에 내가 가진 재화가 생기게 되면 바꿔
        {
            errorScreen.SetActive(true);
            errorScreen.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(moneylackless);
            EndShopping();
            return;
        }

        Item[] items = ShopManager.instance.BuyItem(goodsToBuy, (int)slider.value);

        for (int i = 0; i < InventoryManager.instance.openInventoryList.Count; i++)
        {
            for (int j = 0; j < items.Length; j++)
            {
                if (InventoryManager.instance.openInventoryList[i].SetItem(items[j]) == false)
                {
                    errorScreen.SetActive(true);
                    errorScreen.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(spacelackless);
                    EndShopping();
                    return;
                }
                else
                {
                    print("코코다요");
                    print((int)slider.value);
                    goodsToBuy.itemAmount -= (int)slider.value;
                    ShopManager.instance.SaveShop();
                    ShopManager.instance.LoadShop();
                    EndShopping();
                }
            }
        }
    }

    public void Init(ShopGoods shopGoods, Sprite sprite, Vector2 imageSize, Vector3 rotation, int itemAmount, GameObject errorScreen)
    {
        this.errorScreen = errorScreen;
        goodsToBuy = shopGoods;
        image.sprite = sprite;
        image.GetComponent<RectTransform>().sizeDelta = imageSize;
        image.GetComponent<RectTransform>().eulerAngles = rotation;
        amount = itemAmount;
        slider.maxValue = amount;
        slider.value = 1;
        inputField.text = "1";
        valueTxt.SetText((goodsToBuy.Value * amount).ToString());
        maxAmountTxt.SetText($"/<size=35><sub>{amount}</sub></size>");
        valueTxt.SetText(shopGoods.Value.ToString());

        inputField.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.RemoveAllListeners();
        valueDownBtn.onClick.RemoveAllListeners();
        valueUpBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();

        inputField.onValueChanged.AddListener(OnValueChange);
        slider.onValueChanged.AddListener(OnSliderValueChange);
        valueUpBtn.onClick.AddListener(() =>
        {
            int a = (int)slider.value;
            a = Mathf.Clamp(a, 1, amount);
            OnValueChange((a + 1).ToString());
        });
        valueDownBtn.onClick.AddListener(() =>
        {
            int a = (int)slider.value;
            a = Mathf.Clamp(a, 1, amount);
            OnValueChange((a - 1).ToString());
        });
        buyBtn.onClick.AddListener(() =>
        {
            print("h");
            print(goodsToBuy);
            BuyItem(goodsToBuy);
        });
        cancelBtn.onClick.AddListener(EndShopping);
        ShopManager.instance.isBuying = true;
        transform.SetAsLastSibling();
    }
}