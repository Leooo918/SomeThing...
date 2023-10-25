using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    private Animator animator = null;
    private PlayerInput brain = null;

    private void Awake()
    {
        brain = GetComponentInParent<PlayerInput>();
        animator = GetComponent<Animator>();

        brain.onMouseMove += FaceDir;
        brain.onMove += MoveAnimation;
    }

    private void FaceDir(Vector2 mouseDir)
    {
        Vector3 crossVec = Vector3.Cross((Vector3)mouseDir, Vector3.forward);

        if (Vector3.Dot(crossVec, Vector3.up)   > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void MoveAnimation(Vector2 moveDir)
    {
        if(moveDir.magnitude > 0)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }


}
