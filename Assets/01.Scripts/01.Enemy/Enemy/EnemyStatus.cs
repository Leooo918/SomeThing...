using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    private EnemyBrain brain = null;
    private EnemyMove move = null;
    private AIAttack aiAttack = null;
    public EnemyBoxSO boxSO { get; private set; }
    private EnemyContents enemyContents = new EnemyContents();

    public string enemyName;
    public float maxhp { get; private set; }
    public  float maxSpeed { get; private set; }
    public float attack { get; private set; }
    public float speed { get; private set; }
    public float hp { get; private set; }
    public float stunTime { get; private set; }

    public bool isStun { get; private set; }
    public bool canAttack { get; private set; }
    public bool canUseSkill { get; private set; }


    private void Awake()
    {
        transform.GetComponent<Collider2D>().enabled = true;
    }

    public void Damaged(float value, Vector2 hitPoint)
    {
        hp -= value;
        CameraManager.instance.ShakeCam(5, 5, 0.05f);
        move.KnockBack(hitPoint, 0.1f);

        if (hp <= 0)
        {
            transform.GetComponent<Collider2D>().enabled = false;
            brain.Die();
        }
    }

    public void Stun(float time)
    {
        stunTime = time;
        StartCoroutine("StunRoutine");
    }

    IEnumerator StunRoutine()
    {
        isStun = true;
        yield return new WaitForSeconds(stunTime);
        isStun = false;
    }

    public void Init(EnemySO enemySO)
    {
        brain = GetComponent<EnemyBrain>();
        move = GetComponent<EnemyMove>();
        aiAttack = GetComponent<AIAttack>();

        brain.onDie += () =>
        {
            GameObject box = Instantiate(GameManager.instance.boxPf);
            box.GetComponent<BoxScripts>().Init(boxSO);

            box.transform.localScale = new Vector3(1, 1, 1);
            box.transform.eulerAngles = transform.eulerAngles;
            box.transform.position = transform.position;
        };

        for (int i = 0; i < enemySO.enemyContents.Length; i++)
        {
            if (enemySO.enemyContents[i].name == enemyName)
            {
                enemyContents = enemySO.enemyContents[i];
                boxSO = enemyContents.box;

                maxhp = enemyContents.maxHp;
                attack = enemyContents.maxAttack;
                maxSpeed = enemyContents.maxSpeed;

                hp = maxhp;
                speed = maxSpeed;

                aiAttack.SetCoolTime(enemyContents.attackCool, enemyContents.skillCool);
            }
        }
    }
}