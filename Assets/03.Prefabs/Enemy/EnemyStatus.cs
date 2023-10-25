using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    private EnemyBrain brain = null;
    public EnemyBoxSO boxSO { get; private set; }
    [SerializeField] private EnemySO enemySO = null;
    private EnemyContents enemyContents = new EnemyContents();

    [SerializeField] private string enemyName = "";
    private float maxhp = 1000;
    private float maxSpeed = 0;
    private float attack = 0;
    private float speed = 0;
    private float hp = 0;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();

        brain.onDie += () =>
        {
            GameObject box = Instantiate(GameManager.instance.boxPf);
            box.GetComponent<BoxScripts>().Init(boxSO);

            box.transform.localScale = new Vector3(1, 1, 1);
            box.transform.eulerAngles = transform.eulerAngles;
            box.transform.position = transform.position;
        };

        Init(enemySO);                                  //이거도 임시 나중에 지우고 Enemy매니저에서 실행해줘
    }

    private void OnEnable()
    {
        transform.GetComponent<Collider2D>().enabled = true;
        hp = maxhp;
        speed = maxSpeed;
    }

    public void Damaged(float value)
    {
        hp -= value;

        if (hp <= 0)
        {
            transform.GetComponent<Collider2D>().enabled = false;
            brain.Die();
        }
    }

    public void ReduceMaxHp(float value)
    {

    }

    public void Init(EnemySO enemySO)
    {
        for (int i = 0; i < enemySO.enemyContents.Length; i++)
        {
            if (enemySO.enemyContents[i].name == enemyName)
            {
                this.enemySO = enemySO;
                enemyContents = enemySO.enemyContents[i];
                boxSO = enemyContents.box;

                maxhp = enemyContents.maxHp;
                attack = enemyContents.maxAttack;
                maxSpeed = enemyContents.maxSpeed;
            }
        }
    }
}