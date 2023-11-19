using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private EnemyStatus status;
    private EnemyBrain brain;
    private EnemyRenderer enemyRenderer;
    private Rigidbody2D rb;

    private Vector2 moveDir;
    private Vector2 knockDir;

    private float moveSpeed = 5f;
    private float knockSpeed = 10f;
    private float knockTime = 0.1f;

    private bool isKnockBacking = false;
    private bool canNotMove = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        status = GetComponent<EnemyStatus>();
        brain = GetComponent<EnemyBrain>();
        enemyRenderer = GetComponent<EnemyRenderer>();

        brain.onMove += Move;
    }

    private void Update()
    {
        if (canNotMove == false && isKnockBacking == false)
        {
            rb.velocity = moveDir * moveSpeed;
        }

        if (isKnockBacking == true)
        {
            rb.velocity = knockDir * knockSpeed;
        }
    }

    public void Dash(Vector2 dir, float time, float speed)
    {
        this.knockTime = time;
        knockSpeed = speed;
        knockDir = dir.normalized;
        StopCoroutine("KnockRoutine");
        StartCoroutine("KnockRoutine");
    }

    public void KnockBack(Vector2 hitpoint, float knockTime)
    {
        this.knockTime = knockTime;
        knockSpeed = Mathf.Clamp(hitpoint.magnitude, 3, 5) * 3;
        knockDir = (Vector2)transform.position - hitpoint;
        StopCoroutine("KnockRoutine");
        StartCoroutine("KnockRoutine");
    }

    IEnumerator KnockRoutine()
    {
        isKnockBacking = true;
        yield return new WaitForSeconds(knockTime);
        isKnockBacking = false;
    }

    public void Move(Vector2 dir)
    {
        moveDir = dir;
    }

    public void Init(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
}
