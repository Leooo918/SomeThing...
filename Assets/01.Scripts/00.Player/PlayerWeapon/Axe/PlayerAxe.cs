using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxe : Weapon
{
    private Animator animator = null;
    private DamageSource damageSource = null;
    private PlayerStatus status = null;
    private Collider2D coll = null;
    private Rigidbody2D rigid = null;

    private bool isThrowing = false;
    [SerializeField] private float throwSpeed = 10f;
    private float flyingTime = 0.5f;
    private Vector2 throwDir;

    private bool isAxeStoped = false;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        damageSource = GetComponent<DamageSource>();
        status = GetComponentInParent<PlayerStatus>();
        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        coll.enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerAction.onAttackButtonPress += OnAttack;
        playerAction.onUseSkill += OnUseSkill;
        playerAction.onMouseMove += PlayerDir;
    }

    protected void OnDisable()
    {
        if (playerAction != null)
        {
            playerAction.onAttackButtonPress -= OnAttack;
            playerAction.onUseSkill -= OnUseSkill;
            playerAction.onMouseMove -= PlayerDir;
        }
    }

    protected override void Update()
    {
        base.Update();

        transform.Find("Axe").localPosition = new Vector3(0, 0, 0);

        if (isThrowing == true)
        {
            float rot = transform.eulerAngles.z + Time.deltaTime * -1000;

            transform.eulerAngles = new Vector3(0, 0, rot);
            rigid.velocity = throwDir.normalized * throwSpeed;
            print($"{rot} {throwDir} {throwSpeed}");
        }
        //else
        //{
        //    rigid.velocity = new Vector3(0, 0, 0);
        //}

        if (isAxeStoped == false && isThrowing == false)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }

        if (isAxeStoped == true)
        {
            rigid.velocity = Vector2.zero;
            Collider2D coll = Physics2D.OverlapCircle(transform.position, 1f);

            if (coll != null)
            {
                if (coll.TryGetComponent<PlayerStatus>(out PlayerStatus status))
                {
                    ItemSO itemSO = GameManager.instance.itemSO;
                    Item item = null;

                    for (int i = 0; i < itemSO.items.Count; i++)
                    {
                        if (itemSO.items[i].itemName == itemName)
                        {
                            item = Instantiate(itemSO.items[i].pfItem, GameObject.Find("Canvas").transform).GetComponent<Item>();

                            RectTransform r = item.GetComponent<RectTransform>();

                            r.localPosition = new Vector3(1, 1, 1);
                            r.anchoredPosition3D = new Vector3(0, 0, 0);

                            item.Init(itemSO.items[i], 0);
                            item.GetComponent<ItemWeapon>().Init(durability);
                            status.GetItem(item, durability);
                            isAxeStoped = false;

                            Destroy(this.gameObject);
                            break;
                        }
                    }

                }
            }

        }
    }

    protected override void OnAttack()
    {
        if (isAttackCool == true || isUsingSkill == true || isAttaking == true)
        {
            return;
        }

        base.OnAttack();

        StartCoroutine(BooleanAnimation("Attack"));
    }

    protected override void OnUseSkill()
    {
        base.OnUseSkill();

        if (playerAction != null)       //playerAction이 널이 아니라면
        {
            playerAction.onAttackButtonPress -= OnAttack;
            playerAction.onUseSkill -= OnUseSkill;
            playerAction.onMouseMove -= PlayerDir;
        }

        coll.enabled = true;            //콜라이더를 켜고
        playerStatus.GetCurWeaponSlot().UnEquipWithOutDestroyGameObject();  //도끼를 장착 해제해
        transform.parent = null;        //부모를 없애고?
        throwDir = playerMove.MouseDir; //마우스방향으로 날라갈 준비
        OnUseSubSkill();                //보조스킬도 사용해
        StartCoroutine("ThrowAxeRoutine");  //

    }

    protected override void OnUseSubSkill()
    {
        status.SpeedDown(-3, 2);
    }

    public void OnAttackStart()
    {
        coll.enabled = true;
        isAttaking = true;
        damageSource.damage = damage;
        playerAction.onMouseMove -= PlayerDir;
    }
    public void OnAttackEnd()
    {
        coll.enabled = false;
        isAttaking = false;
        playerAction.onMouseMove += PlayerDir;

        AttackCoolDown();
    }

    private void PlayerDir(Vector2 dir)
    {
        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        float rotation = Mathf.LerpAngle(transform.eulerAngles.z, rot, 1f);
        transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    IEnumerator BooleanAnimation(string parmName)
    {
        animator.SetBool(parmName, true);
        yield return null;
        yield return null;
        yield return null;
        animator.SetBool(parmName, false);
    }

    IEnumerator ThrowAxeRoutine()
    {
        print("제발");
        isThrowing = true;
        yield return new WaitForSeconds(flyingTime);
        isThrowing = false;
        isAxeStoped = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isThrowing == true && collision.CompareTag("Wall"))
        {
            isThrowing = false;
            isAxeStoped = true;
        }
    }
}
