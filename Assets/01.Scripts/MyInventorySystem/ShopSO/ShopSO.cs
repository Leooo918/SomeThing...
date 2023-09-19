using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShopStruct
{
    public int shopLv;
    public GameObject pfShop;
}


[CreateAssetMenu(menuName = "SO/Shop")]
public class ShopSO : ScriptableObject
{
    public ShopStruct[] shops;
    public ShopGoodsSO shopGoodsSO;
}
