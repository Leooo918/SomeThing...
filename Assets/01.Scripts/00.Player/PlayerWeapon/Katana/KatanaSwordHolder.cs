using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaSwordHolder : MonoBehaviour
{
    private PlayerInput input = null;
    private Transform sword = null;
    private bool isAnchored = false;

    private void Awake()
    {
        input = transform.root.GetComponent<PlayerInput>();
        sword = transform.Find("Sword");
    }

    private void OnDisable()
    {
        UnMouseFollow();
    }

    private void OnDestroy()
    {
        UnMouseFollow();
    }

    private void Update()
    {
        if (isAnchored == true)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            sword.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


    public void MouseFollow()
    {
        input.onMouseMove += PlayerDir;
        isAnchored = false;
    }

    public void UnMouseFollow()
    {
        input.onMouseMove -= PlayerDir;
        isAnchored = true;
    }

    private void PlayerDir(Vector2 dir)
    {
        if (this == null) return;

        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        float rotation = Mathf.LerpAngle(transform.eulerAngles.z, rot, 1f);
        transform.eulerAngles = new Vector3(0,0,rotation);
    }
}
