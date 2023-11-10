using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "SO/Branch")]
public class BranchSO : ScriptSO
{
    [Header("---����---")]
    [Tooltip("����������")]
    public Condition condition;
    [Header("---�ٲ��� ����---")]
    public ScriptSO nextScriptOnTrue;
    public ScriptSO nextScriptOnFalse;
}
