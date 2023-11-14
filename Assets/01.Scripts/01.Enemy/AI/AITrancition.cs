using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITrancition : MonoBehaviour
{
    protected List<AIDecision> _decisionList = new List<AIDecision>();
    public AIState nextState;

    public void SetUp(Transform parentTrm)
    {
        GetComponents<AIDecision>(_decisionList);

        foreach (var dec in _decisionList)
        {
            dec.SetUp(parentTrm);
        }
    }

    public bool CanTransition()
    {
        bool result = false;

        foreach (var dec in _decisionList)
        {
            result = dec.MakeDesition();
            if (dec.isReverse)
                result = !result;

            if (result == false)
                break;
        }

        return result;
    }
}
