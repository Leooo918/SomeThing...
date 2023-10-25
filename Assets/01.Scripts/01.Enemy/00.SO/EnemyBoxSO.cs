using UnityEngine;

[System.Serializable]
public struct BoxContents
{
    public string itemName;
    public int maxItemAmount;
    public float exisistPercentage;
}

[CreateAssetMenu(menuName = "SO/EnemyBox")]
public class EnemyBoxSO : ScriptableObject
{
    public BoxContents[] items;
}
