using UnityEngine;

[System.Serializable]
public struct BoxContents
{
    string itemName;
    int positionX;
    int positionY;
    float rotation;
    int maxItemAmount;
}

[CreateAssetMenu(menuName = "SO/EnemyBox")]
public class EnemyBoxSO : ScriptableObject
{
    public BoxContents[] items;
}
