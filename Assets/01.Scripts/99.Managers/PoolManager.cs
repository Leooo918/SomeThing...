using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null;

    private PoolSO pools;

    private Dictionary<string, Queue<GameObject>> poolDic = new Dictionary<string, Queue<GameObject>>();

    public void Enqueue(string name, GameObject obj)
    {
        if (poolDic.TryGetValue(name, out Queue<GameObject> q))
        {
            q.Enqueue(obj);
            Debug.Log(q.Count);
            obj.SetActive(false);
            //poolDic.Add(name, q);
        }
    }

    public GameObject Dequeue(string name)
    {
        if (poolDic.TryGetValue(name, out Queue<GameObject> q))
        {
            return q.Dequeue();
        }
        return null;
    }

    public void Init(PoolSO pool, Transform parent)
    {
        this.pools = pool;
        for (int i = 0; i < pools.pools.Length; i++)
        {
            Queue<GameObject> q = new Queue<GameObject>();
            for (int j = 0; j < pools.pools[i].amount; j++)
            {
                GameObject g = Instantiate(pools.pools[i].toPool, parent);
                q.Enqueue(g);
                g.SetActive(false);
            }
            poolDic.Add(pools.pools[i].poolName, q);
        }
    }
}
