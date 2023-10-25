using UnityEngine;

[System.Serializable]
public struct EffectStruct
{
    public string effectName;
    public GameObject effectImage;
    public float effectDuration;
    public float value;
}

[CreateAssetMenu(menuName = "SO/Effects")]
public class EffectSO : ScriptableObject
{
    public EffectStruct[] effects;
}