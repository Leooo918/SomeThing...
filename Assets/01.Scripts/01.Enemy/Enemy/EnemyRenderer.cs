using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyRenderer : MonoBehaviour
{
    private EnemyBrain enemyBrain = null;
    private EnemyStatus enemyStatus = null;
    private Renderer render = null;
    private SpriteRenderer spriteRenderer = null;

    private void Awake()
    {
        enemyBrain = GetComponentInParent<EnemyBrain>();
        enemyStatus = GetComponentInParent<EnemyStatus>();
        render = GetComponent<Renderer>();

        enemyBrain.onDie += OnDisolve;
    }

    private void OnDisolve()
    {
        print("코코다요");
        StartCoroutine("DisolveRoutine");
    }

    IEnumerator DisolveRoutine()
    {
        float a = 0;
        while(render.material.GetFloat("_DisolveValue") > 0)
        {
            a += Time.deltaTime / 1.2f;
            render.material.SetFloat("_DisolveValue", Mathf.Lerp(1, 0, a));
            yield return null;
        }
        PoolManager.instance.Enqueue(enemyStatus.enemyName,enemyStatus.gameObject);
    }
}
