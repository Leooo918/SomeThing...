using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Camera mainCam = null;

    public event Action<Vector2> onMove = null;
    public event Action<Vector2> onMouseMove = null;
    public Action onAttackButtonPress = null;
    public Action onAttackButtonRelease = null;
    public Action onUseSkill = null;
    public event Action onInteraction = null;
    public event Action onHit = null;
    public event Action onOpenInventory = null;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (GameManager.instance.isUIInput == false)
        {
            Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 mouseDir = mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            onMove?.Invoke(moveDir.normalized);
            onMouseMove?.Invoke(mouseDir);

            if (Input.GetMouseButtonDown(0))
            {
                onAttackButtonPress?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                onUseSkill?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                onInteraction?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            onOpenInventory?.Invoke();
        }
    }


}
