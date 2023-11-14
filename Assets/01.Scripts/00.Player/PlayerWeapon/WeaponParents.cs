using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParents : MonoBehaviour
{
    private PlayerInput player = null;
    public Weapon currentWeapon = null;
    public List<Weapon> weaponList = null;
    private int curWeaponNum = 0;

    private void Awake()
    {
        player = GetComponentInParent<PlayerInput>();

        Weapon[] w = GetComponentsInChildren<Weapon>();

        return;
    }

    public void WeaponDirByMove()
    {
        player = GetComponentInParent<PlayerInput>();
        player.onMouseMove -= WeaponDir;
        Debug.Log("어째서..............");
    }
    public void WeaponDirByMouse()
    {
        player = GetComponentInParent<PlayerInput>();
        player.onMouseMove += WeaponDir;
        Debug.Log("코코");
    }

    private void WeaponDir(Vector2 dir)
    {
        Vector3 crossVec = Vector3.Cross(dir, Vector3.forward);
        float dot = Vector3.Dot(crossVec, Vector3.up);

        if (dot > 0) // 왼쪽
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (dot < 0) // 오른쪽
        {
            transform.localScale = new Vector3(1, 1, 1);
        };
    }}
