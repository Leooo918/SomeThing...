using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AIAction
{
    public override void TakeAcion()
    {
        enemyBrain.GetComponent<AIAttack>().Attack(enemyBrain.playerTrm.position);
    }
}
