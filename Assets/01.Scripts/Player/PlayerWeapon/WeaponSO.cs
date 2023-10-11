using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponStruct
{
    public string name;
    public float damageMultiple;
    public Sprite weaponImage;
    public GameObject weaponImageObj;
    public GameObject weapon;
}

[CreateAssetMenu(menuName = "SO/Weapon")]
public class WeaponSO : ScriptableObject
{
    public WeaponStruct[] weapons;
}
