using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "SO/Branch")]
public class BranchSO : ScriptSO
{
    [Header("---조건---")]
    [Tooltip("프리펩으로")]
    public Condition condition;
    [Header("---바꾸지 마요---")]
    public ScriptSO nextScriptOnTrue;
    public ScriptSO nextScriptOnFalse;
}
