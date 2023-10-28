using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Camera mainCam = null;

    public event Action<Vector2> onMove = null;
    public event Action<Vector2> onMouseMove = null;
    public event Action onAttackButtonPress = null;
    public event Action onAttackButtonRelease = null;
    public event Action onUseSkill = null;
    public event Action onUseSubSkill = null;

    public event Action onInteraction = null;
    public event Action onHit = null;
    public event Action onOpenInventory = null;
    public event Action onChangeWeapon = null;

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
            if (Input.GetMouseButtonUp(0))
            {
                onAttackButtonRelease?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                onUseSkill?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                onInteraction?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                onChangeWeapon?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                onUseSubSkill?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            onOpenInventory?.Invoke();
        }
    }


}
