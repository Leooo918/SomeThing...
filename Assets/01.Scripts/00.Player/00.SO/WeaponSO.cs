using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponStatus
{
    public float normalAttackCool;
    public List<float> damage;
    public List<float> skillCool;
    public string weaponExplain;
}

[System.Serializable]
public struct WeaponStruct
{
    public string name;
    public Sprite weaponImage;
    public GameObject weaponImageObj;
    public GameObject weapon;
    public WeaponStatus weaponStatus;
}

[CreateAssetMenu(menuName = "SO/Weapon")]
public class WeaponSO : ScriptableObject
{
    public WeaponStruct[] weapons;
}
