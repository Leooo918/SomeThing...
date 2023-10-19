using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class ItemWeapon : Item
{
    public WeaponSave weaponnData = new WeaponSave();

    protected float durability = 100;   //내구도
    protected float proficiency = 0;    //숙련도

    protected TextMeshProUGUI durabilityText = null;
    protected TextMeshProUGUI proficiencyText = null;

    public float Durability => durability;
    public float Proficiency => proficiency;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
}
