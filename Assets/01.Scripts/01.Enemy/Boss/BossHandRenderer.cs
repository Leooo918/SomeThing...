using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossHandRenderer : MonoBehaviour
{
    private Animator anim = null;
    private SpriteRenderer sr = null;
    private Material mat = null; 

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        mat = sr.material;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Hit();
        }
    }

    public void SetAttackAnim()
    {
        anim.SetTrigger("Attack");
    }

    public void SetSkillAnim()
    {
        anim.SetTrigger("Skill");
    }

    public void SetDieAnim()
    {
        anim.SetTrigger("Die");
    }

    public void Hit()
    {
        StartCoroutine("HitRoutine");
    }

    IEnumerator HitRoutine()
    {
        mat.SetFloat("_LerpValue", 0.3f);
        yield return new WaitForSeconds(0.1f);
        mat.SetFloat("_LerpValue", 0f);
    }
}
