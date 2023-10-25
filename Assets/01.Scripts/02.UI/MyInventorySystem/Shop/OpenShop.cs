using UnityEngine;

public class OpenShop : MonoBehaviour
{
    private Shop assignedShop = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ShopOpen();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShopClose();
        }
    }

    public void ShopOpen()
    {
        assignedShop.gameObject.SetActive(true);
    }

    public void ShopClose()
    {
        assignedShop.gameObject.SetActive(false);
    }

    public void Init(Shop shop)
    {
        assignedShop = shop;
        ShopClose();
    }
}
