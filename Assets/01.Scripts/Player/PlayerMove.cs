using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    private PlayerInput action = null;
    private Rigidbody2D rigid = null;

    private float moveSpeed = 5f;

    private void Awake()
    {
        action = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        action.onMove += Move;
    }

    private void Move(Vector2 moveDir)
    {  
        rigid.velocity = moveDir * moveSpeed;
    }

    public void ChangeSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
}
