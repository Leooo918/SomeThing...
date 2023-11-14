using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    protected EnemyBrain enemyBrain;

    public virtual void SetUp(Transform parentTrm)
    {
        enemyBrain = parentTrm.GetComponent<EnemyBrain>();
    }

    public abstract void TakeAcion();
}
