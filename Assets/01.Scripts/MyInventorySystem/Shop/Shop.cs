using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<ShopGoods> shopGoodses = new List<ShopGoods>();


    public void Init(ShopGoodsSO shopGoodsSO, ItemBuyHelper itemBuyHelper, GameObject errorScreen)
    {
        ShopGoods[] shopGoodses = GetComponentsInChildren<ShopGoods>();


        foreach(ShopGoods shopGoods in shopGoodses)
        {
            this.shopGoodses.Add(shopGoods);
        }

        ShopManager.instance.LoadShop();

        foreach(ShopGoods shopGoods in shopGoodses)
        {
            shopGoods.Init(shopGoodsSO, itemBuyHelper, errorScreen);
        }

        FindAnyObjectByType<OpenShop>().Init(this);
    }
}