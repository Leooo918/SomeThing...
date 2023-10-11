using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Normal
{
    public string itemName;
    public Vector2Int size;
    public GameObject pfItem;
    public ItemType itemType;
    public int itemValue;
}

[System.Serializable]
public struct Expendable
{
    public string itemName;
    public Vector2Int size;
    public GameObject pfItem;
    public ItemType itemType;
    public int maxItemNum;
    public int itemValue;
}

[CreateAssetMenu(menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    public List<Normal> items = new List<Normal>();
    public List<Expendable> expendableItems = new List<Expendable>();
}
