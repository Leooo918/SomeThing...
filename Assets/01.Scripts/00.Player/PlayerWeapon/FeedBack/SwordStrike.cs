using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordStrike : MonoBehaviour
{
    private Vector3 moveDir;
    [SerializeField] private float speed = 3f;

    private void Update()
    {
        transform.position += moveDir * Time.deltaTime * speed;
    }

    public void SetDir(Vector2 dir)
    {
        float rot = Mathf.Atan2(dir.y, dir.x);
        transform.eulerAngles = new Vector3(0,0,rot);
        moveDir = dir;
        Destroy(gameObject, 1f);
    }

}
