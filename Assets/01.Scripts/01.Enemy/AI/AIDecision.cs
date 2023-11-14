using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIDecision : MonoBehaviour
{
    public bool isReverse;
    protected EnemyBrain _enemyBrain;

    public virtual void SetUp(Transform parentTrm)
    {
        _enemyBrain = parentTrm.GetComponent<EnemyBrain>();
    }

    public abstract bool MakeDesition();
}
