using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : AIAction
{
    public override void TakeAcion()
    {
        Debug.Log("나는 아무것도 하지 않다 왜냐면 Idle상태기 때문이지 후훗");
    }
}
