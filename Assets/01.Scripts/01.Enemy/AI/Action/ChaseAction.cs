using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAction : AIAction
{
    public override void TakeAcion()
    {
        Debug.Log("��ȣ");
        enemyBrain.Move(enemyBrain.playerTrm.position);
    }
}
