using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnemyBrain : MonoBehaviour
{
    public Action<Vector2> onMove = null;
    public Action onAttack = null;
    public Action onDie = null;

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
}
