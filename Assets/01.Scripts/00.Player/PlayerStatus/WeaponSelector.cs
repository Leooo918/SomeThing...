using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    private PlayerStatus playerStatus = null;

    private void Awake()
    {
        playerStatus = GetComponentInParent<PlayerStatus>();
        WeaponSelectEnd(0);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            WeaponSelectStart();
        }
    }   
    
    public void WeaponSelectStart()
    {
        Time.timeScale = 0.1f;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void WeaponSelectEnd(int num)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //playerStatus.OnChangeWeapon(num);
    }

}
