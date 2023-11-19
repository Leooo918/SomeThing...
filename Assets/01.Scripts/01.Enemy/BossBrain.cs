using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBrain : MonoBehaviour
{
    private EnemyBrain head;
    private EnemyBrain leftHand;
    private EnemyBrain rightHand;

    private EnemyStatus headStat;
    private EnemyStatus leftHandStat;
    private EnemyStatus rightHandStat;

    private void Awake()
    {
        head = transform.Find("Head").GetComponent<EnemyBrain>();
        leftHand = transform.Find("LeftHand").GetComponent<EnemyBrain>();
        rightHand = transform.Find("RightHand").GetComponent<EnemyBrain>();

        headStat = head.GetComponent<EnemyStatus>();
        leftHandStat = leftHand.GetComponent<EnemyStatus>();
        rightHandStat = rightHand.GetComponent<EnemyStatus>();
    }

    private void Start()
    {
        head.Init(this);
        leftHand.Init(this);
        rightHand.Init(this);
    }

    private void Update()
    {

    }

    public void HandDestroyed()
    {
        headStat.Damaged(leftHandStat.maxhp, transform.position);
    }
}