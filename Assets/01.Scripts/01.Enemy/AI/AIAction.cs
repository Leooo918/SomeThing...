using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    protected EnemyBrain _enemyBrain;

    public virtual void SetUp(Transform parentTrm)
    {
        _enemyBrain = parentTrm.GetComponent<EnemyBrain>();
    }

    public abstract void TakeAcion();
}
