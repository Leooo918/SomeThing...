using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyContents
{
    public string name;
    public float maxHp;
    public float maxAttack;
    public float maxSpeed;

    public float attackCool;
    public float skillCool;
    
    public EnemyBoxSO box;
}

[CreateAssetMenu(menuName = "SO/Enemy")]
public class EnemySO : ScriptableObject
{
    public EnemyContents[] enemyContents;
}
