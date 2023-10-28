using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    private bool isDashing = false;

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
    }

    private void Move(Vector2 moveDir)
    {
        if (isDashing) return;
        rigid.velocity = moveDir * status.GetPlayerSpeed();
    }
    private void GetMouseDir(Vector2 mouseDir)
    {
        this.mouseDir = mouseDir;
    }

    public void Flash()
    {
        seq = DOTween.Sequence();

        seq.Append(playerRenderer.transform.DOScale(new Vector3(0, 0, 0), 0.15f))
            .AppendCallback(() =>
            {
                GameObject particle = Instantiate(flashParticle);
                particle.transform.position = transform.position;

                for (int i = 4; i > 0; i--)
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
        StartCoroutine(DashRoutine(time));
    }

    IEnumerator DashRoutine(float time)
    {
        isDashing = true;
        yield return new WaitForSeconds(time);
        isDashing = false;
    }
}
