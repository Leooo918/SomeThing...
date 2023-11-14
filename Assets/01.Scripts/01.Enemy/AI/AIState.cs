using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    protected List<AIAction> _actionList = new List<AIAction>();
    protected List<AITrancition> _transitionList = new List<AITrancition>();

    protected EnemyBrain _brain;

    public void SetUp(Transform parent)
    {
        _brain = parent.GetComponent<EnemyBrain>();
        GetComponents<AIAction>(_actionList);
        _actionList.ForEach(a => a.SetUp(parent));

        GetComponentsInChildren<AITrancition>(_transitionList);
        _transitionList.ForEach(b => b.SetUp(parent));
    }

    public void UpdateState()
    {
        foreach (var action in _actionList)
        {
            action.TakeAcion();
        }
        foreach (var transition in _transitionList)
        {
            if (transition.CanTransition())
            {
                _brain.ChangeState(transition.nextState);
            }
        }
    }
}
