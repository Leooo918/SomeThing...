using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class EnemyBrain : MonoBehaviour
{
    private EnemyStatus enemyStatus;
    private EnemyMove enemyMove;
    private EnemySpawner spawner;
    private AIAttack attack;
    private EnemyRenderer render;

    public Transform playerTrm { get; private set; }

    public Action<Vector2> onMove = null;
    public Action onAttack = null;
    public Action onDie = null;

    private AIState currentState;

    private void Start()
    {
        enemyStatus = GetComponent<EnemyStatus>();
        enemyMove = GetComponent<EnemyMove>();
        attack = GetComponent<AIAttack>();
        render = transform.Find("Sprite").GetComponent<EnemyRenderer>();
        currentState = transform.Find("AI/IdleState").GetComponent<AIState>();

        playerTrm = GameManager.instance.player;

        transform.Find("AI").GetComponentsInChildren<AIState>()
            .ToList()
            .ForEach(s => s.SetUp(transform));

        enemyStatus.Init(GameManager.instance.enemySO);
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    public void Move(Vector2 moveDir)
    {
        onMove?.Invoke(moveDir);
    }

    public void Die()
    {
        onDie?.Invoke();
        spawner.EnemyDead();
    }

    public void ChangeState(AIState nextState)
    {
        currentState = nextState;
    }

    public void Init(EnemySpawner spawner)
    {
        enemyStatus = GetComponent<EnemyStatus>();
        enemyMove = GetComponent<EnemyMove>();
        attack = GetComponent<AIAttack>();
        render = transform.Find("Sprite").GetComponent<EnemyRenderer>();
        currentState = transform.Find("AI/IdleState").GetComponent<AIState>();

        transform.Find("AI").GetComponentsInChildren<AIState>()
            .ToList()
            .ForEach(s => s.SetUp(transform));

        this.spawner = spawner;
        enemyStatus.Init(GameManager.instance.enemySO);
    }
}
