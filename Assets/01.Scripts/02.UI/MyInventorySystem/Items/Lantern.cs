 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : ItemWeapon
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void DurabilityDown(float value)
    {
        durability -= value;

        if(durability <= 0)
        {
            Destroy(gameObject);
        }
    }
}
