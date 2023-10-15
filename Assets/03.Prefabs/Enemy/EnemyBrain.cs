using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnemyBrain : MonoBehaviour
{
    public Action<Vector2> onMove = null;
    public Action onAttack = null;
    public Action onDie = null;

    private void Awake()
    {
        onDie += () =>
        {
            GameObject box = Instantiate(GameManager.instance.boxPf);

            box.transform.localScale = new Vector3(1, 1, 1);
            box.transform.eulerAngles = transform.eulerAngles;
            box.transform.position = transform.position;
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            onDie?.Invoke();
        }
    }

    public void Move(Vector2 moveDir)
    {
        onMove?.Invoke(moveDir);
    }

    public void Die()
    {
        onDie?.Invoke();
    }
}
