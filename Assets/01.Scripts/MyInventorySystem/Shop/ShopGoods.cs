using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopGoods : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler,IPointerExitHandler
{
    public string goodsName;
    private GameObject soldOutImage = null;
    private GameObject errorScreen = null;
    private RectTransform rectTransform = null;
    private ItemBuyHelper itemBuyHelper = null;
    private TextMeshProUGUI amountText = null;
    private Sprite sprite = null;
    private int value = 0;
    public int itemAmount = 0;

    private bool checkSoldOut = false;

    public int Value => value; 

    private void Awake()
    {
        soldOutImage = transform.Find("SoldOut").gameObject;
        sprite = transform.Find("Sprite").GetComponent<Image>().sprite;
        rectTransform = GetComponent<RectTransform>();
        amountText = transform.Find("ItemAmountTxt").GetComponent<TextMeshProUGUI>();
        soldOutImage.SetActive(false);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        ShopManager.instance.isBuying = true;
        itemBuyHelper.gameObject.SetActive(true);
        itemBuyHelper.Init(this, sprite, rectTransform.sizeDelta,rectTransform.eulerAngles, itemAmount, errorScreen);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void SetText()
    {
        amountText.SetText(itemAmount.ToString());
    }

    public void SoldOut()
    {
        checkSoldOut = true;
        soldOutImage.SetActive(checkSoldOut);
    }

    public void Init(ShopGoodsSO shopGoodsSO, ItemBuyHelper itemBuyHelper, GameObject errorScreen)
    {
        this.itemBuyHelper = itemBuyHelper;

        for(int i =0; i < shopGoodsSO.shopGoods.Count; i++)
        {
            if(shopGoodsSO.shopGoods[i].goodsName == goodsName)
            {
                value = shopGoodsSO.shopGoods[i].value;
            }
        }

        this.errorScreen = errorScreen;
        checkSoldOut = false;
        soldOutImage.SetActive(checkSoldOut);
    }

}
