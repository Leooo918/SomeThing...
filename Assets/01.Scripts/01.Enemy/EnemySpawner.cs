using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnerSO spawner;
    public Queue<string> remainEnemyQueue = new Queue<string>();
    public int spawnedEnemy = 0;
    [SerializeField] private int minSpawnNum = 0;
    [SerializeField] private int maxSpawnNum = 7;

    [SerializeField] private float spawnRange = 4f;
    [SerializeField] private float minPlayerDetectRange = 15f;
    [SerializeField] private float maxPlayerDectecRange = 20f;

    private void Awake()
    {
        for (int i = 0; i < spawner.toSpawn.Length; i++)
        {
            remainEnemyQueue.Enqueue(spawner.toSpawn[i]);
        }
    }


    private void Update()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, maxPlayerDectecRange, LayerMask.GetMask("Player"));
        if (coll != null && spawnedEnemy <= 0)
        {
            if ((coll.transform.position - transform.position).magnitude >= minPlayerDetectRange)
            {
                SpawnEnemy();
            }
        }
    }

    public void SpawnEnemy()
    {
        int spawnNum = Random.Range(minSpawnNum, maxSpawnNum + 1);
        for (int i = 0; i < spawnNum; i++)
        {
            if (remainEnemyQueue.TryDequeue(out string e))
            {
                print(e);
                GameObject g = PoolManager.instance.Dequeue(e);
                g.SetActive(true);
                g.transform.position = GetSpawnPos();
                g.GetComponent<EnemyBrain>().Init(this);
                spawnedEnemy++;
            }
        }
    }

    public void EnemyDead()
    {
        spawnedEnemy--;
    }

    private Vector2 GetSpawnPos()
    {
        Vector2 pos = transform.position + new Vector3(Random.Range(-spawnRange / 2, spawnRange / 2), Random.Range(-spawnRange / 2, spawnRange / 2), 0);

        return pos;
    }




#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange, spawnRange, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxPlayerDectecRange);
            Gizmos.DrawWireSphere(transform.position, minPlayerDetectRange);
        }
    }
#endif
}
