using System.Collections;
using System;
using UnityEngine;

public class ShelterPlayer : MonoBehaviour
{
    public delegate void onMoveComplete();
    private onMoveComplete onComplete;
    private bool isMoving = false;
    private Vector2 targetPos;
    private Animator animator = null;

    private float speed = 3f;

    private void Awake()
    {
        animator = transform.Find("PlayerSprite").GetComponent<Animator>();
    }

    private void Update()
    {
        if (isMoving == true)
        {
            animator.SetBool("Move", true);
            transform.position += ((Vector3)targetPos - transform.position).normalized * speed * Time.deltaTime;

            if (((Vector3)targetPos - transform.position).x > 0)
                transform.localScale = new Vector3(1.5f, 1.5f, 1);
            else
                transform.localScale = new Vector3(-1.5f, 1.5f, 1);


            if ((transform.position - (Vector3)targetPos).magnitude <= 0.1f)
            {
                isMoving = false;
                onComplete();
            }
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }

    public void Move(Vector2 position, onMoveComplete onComplete)
    {
        Debug.Log("움직이기!!!");
        isMoving = true;
        targetPos = position;
        this.onComplete = onComplete;
    }
}
