using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Option")]
public class OptionSO : ScriptSO
{
    public List<string> options = new List<string>();
    public GameObject optionPf;
    [Header("---�ٲ��� ����---")]
    public ScriptSO[] nextScriptsByOption = new ScriptSO[3];
}
