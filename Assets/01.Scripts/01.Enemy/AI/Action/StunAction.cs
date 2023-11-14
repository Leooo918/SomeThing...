using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAction : AIAction
{
    [SerializeField] private GameObject stunEffect;
    private EnemyStatus status;

    private void Awake()
    {
        status = GetComponentInParent<EnemyStatus>();
    }

    public override void TakeAcion()
    {
        Debug.Log("���� ����");
    }

    private void Update()
    {
        stunEffect.SetActive(status.isStun);
    }
}