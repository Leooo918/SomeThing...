using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShelterStoreageObject : ShelterObject
{
    private Animator anim = null;
    private OpenInventory openInventory = null;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
        openInventory = GetComponent<OpenInventory>();
    }


    public override void UseObject()
    {
        OnPointerExit();
        anim.SetTrigger("OpenChest");
        StartCoroutine("OpenInventory");
    }

    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.6f);
        player.GetComponent<OpenInventory>().InventoryOpen();
        openInventory.InventoryOpen();
        openInventory.MyInventory.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            openInventory.InventoryClose();
            player.GetComponent<OpenInventory>().InventoryClose();
            anim.SetTrigger("CloseChest");
            shelter.isInteracting = true;
        });
    }
}
