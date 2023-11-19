using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class EnemyBrain : MonoBehaviour
{
    private EnemyStatus enemyStatus;
    private EnemyMove enemyMove;
    private BossBrain brain;
    private EnemySpawner spawner;
    private AIAttack attack;
    private EnemyRenderer render;
    private NavAgent agent;

    public Transform playerTrm { get; private set; }

    public Action<Vector2> onMove = null;
    public Action onAttack = null;
    public Action onDie = null;

    private AIState currentState;

    private Vector3Int nextPosition;

    private void Awake()
    {
        enemyStatus = GetComponent<EnemyStatus>();
        enemyMove = GetComponent<EnemyMove>();
        attack = GetComponent<AIAttack>();
        agent = GetComponent<NavAgent>();
        render = transform.Find("Sprite").GetComponent<EnemyRenderer>();
    }

    private void Start()
    {
        playerTrm = GameManager.instance.player;
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    public void Move(Vector2 targetPosition)
    {
        Vector3Int destination = MapManager.Instance.GetTilePos(targetPosition);
        agent.Destination = destination; //경로 설정을 시작할 거고

        Vector3 nextWorld = MapManager.Instance.GetWorldPosition(nextPosition);

        if (Vector3.Distance(nextWorld, transform.position) < 0.2f)
        {
            if (agent.GetNextPath(out Vector3Int next))
            {
                nextPosition = next;
            }
            else
            {
                onMove?.Invoke(Vector2.zero);
            }
        }
        else
        {
            Vector2 dir = (nextWorld - transform.position).normalized;
            onMove?.Invoke(dir);
            //적이 바라보는 방향
        }
    }

    public void Stop()
    {
        onMove?.Invoke(Vector2.zero);
    }

    public void Die()
    {
        onDie?.Invoke();
        if (spawner != null)
        {
            spawner.EnemyDead();
        }
        else if(brain != null && enemyStatus.enemyName == "BossHand")
        {
            brain.HandDestroyed();
        }
    }

    public void ChangeState(AIState nextState)
    {
        currentState = nextState;
    }

    public void Init(BossBrain bossBrain)
    {
        brain = bossBrain;
        currentState = transform.Find("AI/IdleState").GetComponent<AIState>();

        transform.Find("AI").GetComponentsInChildren<AIState>()
                    .ToList()
                    .ForEach(s => s.SetUp(transform));

        enemyStatus.Init(GameManager.instance.enemySO);
        nextPosition = MapManager.Instance.GetTilePos(transform.position);
    }

    public void Init(EnemySpawner spawner)
    {
        currentState = transform.Find("AI/IdleState").GetComponent<AIState>();

        transform.Find("AI").GetComponentsInChildren<AIState>()
            .ToList()
            .ForEach(s => s.SetUp(transform));

        this.spawner = spawner;
        enemyStatus.Init(GameManager.instance.enemySO);
        nextPosition = MapManager.Instance.GetTilePos(transform.position);
    }
}
