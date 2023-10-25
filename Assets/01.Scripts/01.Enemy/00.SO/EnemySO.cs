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
    public EnemyBoxSO box;
}

[CreateAssetMenu(menuName = "SO/enemy")]
public class EnemySO : ScriptableObject
{
    public EnemyContents[] enemyContents;
}
