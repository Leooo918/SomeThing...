using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InventoryStruct
{
    public string inventoryName;
    public int width;
    public int height;
    public float slotSize;
    public GameObject pfInvnetory;
    public GameObject pfSlot;
}

[CreateAssetMenu(menuName = "SO/Inventory")]
public class InventorySO : ScriptableObject
{
    public InventoryStruct[] inventoryStructs;
}
