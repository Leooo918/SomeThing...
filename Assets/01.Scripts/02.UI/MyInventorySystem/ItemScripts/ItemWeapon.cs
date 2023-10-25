using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class ItemWeapon : Item
{
    protected float durability = 100;   //내구도
    protected float proficiency = 0;    //숙련도
    protected int proficiencyLv = 0;


    protected Image durabilityValue = null;
    protected TextMeshProUGUI proficiencyTxt = null;

    public float Durability => durability;
    public float Proficiency => proficiency;
    public int ProficiencyLv => proficiencyLv;

    protected override void Awake()
    {
        base.Awake();
        durabilityValue = transform.Find("WeaponStatus/DurabilityBackground/Value").GetComponent<Image>() ;
        proficiencyTxt = transform.Find("WeaponStatus/Lv").GetComponent<TextMeshProUGUI>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void Init(float durability)
    {
        this.durability = durability;

        proficiencyLv = WeaponManager.instance.GetWeaponProficiencyLv(itemName);
        proficiency = WeaponManager.instance.GetWeaponProficiencyValue(itemName);

        durabilityValue.fillAmount = durability / 100f;
        proficiencyTxt.SetText($"Lv.{proficiencyLv}");
    }
}
