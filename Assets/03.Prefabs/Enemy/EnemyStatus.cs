using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    private EnemyBrain brain = null;
    [SerializeField] private EnemyBoxSO boxSO = null;

    private float maxhp = 0;
    private float maxSpeed = 0;
    private float attack = 0;
    private float speed = 0;
    private float hp = 0;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();

    }

    public void Damaged(float value)
    {

        if (value <= 0)
        {
            brain.Die();
        }
    }

    public void ReduceMaxHp(float value)
    {

    }
}