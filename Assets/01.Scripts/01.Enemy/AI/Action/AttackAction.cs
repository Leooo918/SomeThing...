using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AIAction
{
    public override void TakeAcion()
    {
        _enemyBrain.GetComponent<AIAttack>().Attack(_enemyBrain.playerTrm.position);
    }
}
