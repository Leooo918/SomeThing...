using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolingObj
{
    public string poolName;
    public GameObject toPool;
    public int amount;
}

[CreateAssetMenu(menuName = "SO/Pool")]
public class PoolSO : ScriptableObject
{
    public PoolingObj[] pools;
}
