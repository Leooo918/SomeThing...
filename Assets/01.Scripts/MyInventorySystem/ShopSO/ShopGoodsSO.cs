using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Goods
{
    public string goodsName;
    public GameObject pfItem;
    public int value;
    public Vector2Int size;
}


[CreateAssetMenu(menuName = "SO/Goods")]
public class ShopGoodsSO : ScriptableObject
{
    public List<Goods> shopGoods;
}
