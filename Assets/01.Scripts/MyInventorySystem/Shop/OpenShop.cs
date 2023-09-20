using UnityEngine;

public class OpenShop : MonoBehaviour
{
    private Shop assignedShop = null;

    public void ShopOpen()
    {
        assignedShop.gameObject.SetActive(true);
        //assignedShop.Init();
    }

    public void ShopClose()
    {
        assignedShop.gameObject.SetActive(false);
    }

    public void Init(Shop shop)
    {
        assignedShop = shop;
    }
}
