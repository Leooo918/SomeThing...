using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    private Shop assignedShop = null;

    public void ShopOpen()
    {

    }

    public void ShopClose()
    {

    }

    public void Init(Shop shop)
    {
        assignedShop = shop;
    }
}
