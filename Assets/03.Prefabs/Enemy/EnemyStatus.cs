using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyBoxSO boxSO = null;

    public void Damaged(float value)
    {

    }

    public void ReduceMaxHp(float value)
    {
        
    }
}