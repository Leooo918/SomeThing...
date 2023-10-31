using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InvisibleEnemyRenderer : MonoBehaviour
{
    private Renderer render = null;
    private EnemyBrain enemyBrain = null;

    private void Awake()
    {
        enemyBrain = GetComponentInParent<EnemyBrain>();
        render = GetComponent<Renderer>();

    }

    private  void OnInvisible()
    {
        StartCoroutine("InvisibleRoutine");
    }
    private void OnBeNormal()
    {
        StartCoroutine("NotInvisibleRoutine");
    }

    IEnumerator InvisibleRoutine()
    {
        float a = 0;
        while (render.material.GetFloat("_InvisibleValue") > 0)
        {
            a -= Time.deltaTime / 1.2f;
            render.material.SetFloat("_InvisibleValue", Mathf.Lerp(1, 0, a));
            yield return null;
        }
    }

    IEnumerator NotInvisibleRoutine()
    {
        float a = 0;
        while (render.material.GetFloat("_InvisibleValue") > 0)
        {
            a += Time.deltaTime / 1.2f;
            render.material.SetFloat("_InvisibleValue", Mathf.Lerp(1, 0, a));
            yield return null;
        }
    }
}
