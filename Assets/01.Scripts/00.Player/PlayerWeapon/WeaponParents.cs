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
        player = transform.root.GetComponent<PlayerInput>();

        player.onMouseMove += WeaponDir;


        Weapon[] w = GetComponentsInChildren<Weapon>();
        for (int i = 0; i < w.Length; i++)
        {
            weaponList.Add(w[i]);
            w[i].gameObject.SetActive(false);
        }

        if (transform.childCount > curWeaponNum)
        {
            transform.GetChild(curWeaponNum).TryGetComponent<Weapon>(out currentWeapon);
            currentWeapon.gameObject.SetActive(true);
        }
        return;
    }

    private void WeaponDir(Vector2 dir)
    {
        Vector3 crossVec = Vector3.Cross(dir, Vector3.forward);
        float dot = Vector3.Dot(crossVec, Vector3.up);

        if (dot > 0) // ����
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (dot < 0) // ������
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void SetWeaponFollow()
    {
        print(player);
        player.onMouseMove += WeaponFollow;
    }
    public void UnSetWeaponFollow()
    {
        player.onMouseMove -= WeaponFollow;
    }


    private void WeaponFollow(Vector2 dir)
    {
        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, rot);
    }

    private void SwitchWeapon()
    {
        if (transform.childCount == 0) return;

        curWeaponNum++;
        if (curWeaponNum >= transform.childCount)
        {
            curWeaponNum = 0;
        }
        transform.GetChild(curWeaponNum).TryGetComponent<Weapon>(out currentWeapon);
        currentWeapon.gameObject.SetActive(true);
    }
}
