using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShelterShopObject : ShelterObject
{
    private OpenShop shop = null;
    private RectTransform shopUI = null;
    private GameObject cover;

    protected override void Awake()
    {
        base.Awake();
        shop = GetComponent<OpenShop>();
        shopUI = GameObject.Find("ShopUI").GetComponent<RectTransform>();

        shopUI.Find("TalkBtn").GetComponent<Button>().onClick.AddListener(Talk);
        shopUI.Find("ToShopBtn").GetComponent<Button>().onClick.AddListener(OpenShop);
        shopUI.Find("ExitBtn").GetComponent<Button>().onClick.AddListener(CloseUI);
        cover = shopUI.Find("Cover").gameObject;
        cover.SetActive(false);
    }

    public override void UseObject()
    {
        OnInteract();
    }

    private void OnInteract()
    {
        shopUI.DOAnchorPosX(-725f, 1f).SetEase(Ease.OutBounce).OnComplete(() => cover.SetActive(true));
    }

    public void CloseUI()
    {
        cover.SetActive(false);
        shopUI.DOAnchorPosX(-1200f, 0.5f).SetEase(Ease.Linear);
    }

    public void OpenShop()
    {
        CloseUI();
        shop.ShopOpen();
        player.GetComponent<OpenInventory>().InventoryOpen();

        shop.AssignedShop.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        shop.AssignedShop.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            shop.ShopClose();
            player.GetComponent<OpenInventory>().InventoryClose();
            shelter.isInteracting = true;
        });
    }

    public void Talk()
    {
        //일단 1번 인덱스꺼 해주는데 PlayerStatus에 curProgress에 따라 나올거 다르게 해서 해주는거임 ㅇㅋ?

        CloseUI();
        DialogGameManager.instance.StartDialog(1);
    }
}
