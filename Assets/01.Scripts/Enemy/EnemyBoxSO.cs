using UnityEngine;

[System.Serializable]
public struct BoxContents
{
    public string itemName;
    public int maxItemAmount;
    public float exisistPercentage;
}

[System.Serializable]
public struct EnemyBox
{
    public string enemyName;
    public BoxContents[] items;
}

[CreateAssetMenu(menuName = "SO/EnemyBox")]
public class EnemyBoxSO : ScriptableObject
{
    public EnemyBox[] enemyBoxes;
}
