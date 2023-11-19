using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAction : AIAction
{
    public override void TakeAcion()
    {
        Debug.Log("¾ßÈ£");
        enemyBrain.Move(enemyBrain.playerTrm.position);
    }
}
