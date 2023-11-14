using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDecision : AIDecision
{


    public override bool MakeDesition()
    {
        return _enemyBrain.GetComponent<EnemyStatus>().isStun;
    }
}
