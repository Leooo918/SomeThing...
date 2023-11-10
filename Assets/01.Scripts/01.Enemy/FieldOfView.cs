using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float halfAngle = 30f;
    [SerializeField] private float radius = 10f;

    [SerializeField] private Vector2 upVector = Vector2.up;

    private void Update()
    {
        Vector3 vec = Vector2.up;


        Gizmos.DrawRay(new Ray(transform.position, vec));

        if(CheckSide() == true)
        {
            Debug.Log("°¨ÁöµÊ!");
        }
    }

    public bool CheckSide()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Player"));

        foreach (Collider2D coll in colls)
        {
            Vector2 targetPos = coll.transform.position;
            Vector2 targetDir = ((Vector2)transform.position - targetPos).normalized;


            if (Vector2.Dot(upVector, targetDir) > Mathf.Cos(halfAngle))
            {
                Gizmos.DrawRay(new Ray(transform.position, targetPos));
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
