using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    private Sequence seq = null;
    private PlayerInput action = null;
    private PlayerRenderer playerRenderer = null;
    private PlayerStatus status = null;
    private Rigidbody2D rigid = null;
    [SerializeField] private GameObject flashParticle = null;

    private Vector2 mouseDir;
    private Vector2 dashDir;

    private float moveSpeed = 5f;
    private float dashSpeed = 10f;
    private float dashTime;

    private bool isDashing = false;
    public bool canNotMove = false;

    public Vector2 MouseDir => mouseDir;


    private void Awake()
    {
        playerRenderer = GetComponentInChildren<PlayerRenderer>();
        action = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        status = GetComponent<PlayerStatus>();
        action.onMove += Move;
        action.onMouseMove += GetMouseDir;
    }

    private void Update()
    {
        if (isDashing) rigid.velocity = dashDir * dashSpeed;

        if (GameManager.instance.isUIInput == true)
        {
            rigid.velocity = Vector2.zero;
        }
    }

    private void Move(Vector2 moveDir)
    {
        if(canNotMove == true)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        if (isDashing == true) 
        {
            return;
        } 
        rigid.velocity = moveDir * status.GetPlayerSpeed();
    }
    private void GetMouseDir(Vector2 mouseDir)
    {
        this.mouseDir = mouseDir;
    }

    public void Flash(float distance)
    {
        seq = DOTween.Sequence();

        seq.Append(playerRenderer.transform.DOScale(new Vector3(0, 0, 0), 0.15f))
            .AppendCallback(() =>
            {
                GameObject particle = Instantiate(flashParticle);
                particle.transform.position = transform.position;

                for (int i = (int)Mathf.Clamp(mouseDir.magnitude, 0, distance); i > 0; i--)
                {
                    if (Physics2D.OverlapCircle(transform.position + (Vector3)mouseDir.normalized * i / 2, 0.5f) == null)
                    {
                        transform.position += (Vector3)mouseDir.normalized * i / 2;
                    }
                }
            })
            .Append(playerRenderer.transform.DOScale(new Vector3(1, 1, 1), 0.15f));
    }

    public void Dash(float time, float dashSpeed)
    {
        dashDir = mouseDir;
        this.dashSpeed = dashSpeed;
        dashTime = time;
        StartCoroutine("DashRoutine");
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
    }

    public void KnockBack(Vector2 dir)
    {
        dashDir = dir;
        this.dashSpeed = 10 - Mathf.Clamp(dir.magnitude, 0, 8);
        dashTime = 0.05f;
        StopCoroutine("DashRoutine");
        StartCoroutine("DashRoutine");
    }

    public void KnockLong(Vector2 dir, float speed, float time)
    {
        dashDir = dir.normalized;
        dashSpeed = speed;
        dashTime = time;
        StopCoroutine("DashRoutine");
        StartCoroutine("DashRoutine");
    }
}
