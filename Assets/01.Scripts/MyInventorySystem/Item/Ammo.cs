using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : ExpendableItem
{

    protected override void Awake()
    {
        //이거 해야디
        base.Awake();
    }

    protected override void Update()
    {
        //이것도 해야디
        base.Update();
    }
    public override void UseItem(int value)
    {
        base.UseItem(value);


    }
}
