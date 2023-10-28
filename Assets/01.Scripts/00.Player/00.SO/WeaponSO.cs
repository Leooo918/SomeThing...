using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponStatus
{
    public float normalAttackCool;
    public float subSkillCool;
    public List<float> damage;
    public List<float> skillCool;
    public string weaponExplain;
}

[System.Serializable]
public struct WeaponStruct
{
    public string name;
    public Sprite weaponImage;
    public GameObject weapon;
    public GameObject weaponImageObj;
    public GameObject weaponSkillIcon;
    public GameObject weaponSubSkillIcon;
    public WeaponStatus weaponStatus;
}

[CreateAssetMenu(menuName = "SO/Weapon")]
public class WeaponSO : ScriptableObject
{
    public WeaponStruct[] weapons;
}
