using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class WeaponProficiencySave
{
    public List<string> weaponName = new List<string>();
    public List<int> weaponProficiencyLv = new List<int>();
    public List<float> weaponProficiencyProgress = new List<float>();
}


public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance = null;

    private string path = "";

    private WeaponSO weaponSO = null;

    private WeaponProficiencySave saves;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "WeaponProficiency.json");
    }

    public void JsonSave(string itemName, int proficiencyLv, float proficiencyValue)
    {
        for (int i = 0; i < weaponSO.weapons.Length; i++)
        {
            if(saves.weaponName[i] == itemName)
            {
                saves.weaponProficiencyLv[i] = proficiencyLv;
                saves.weaponProficiencyProgress[i] = proficiencyValue;
            }
        }

        string json = JsonUtility.ToJson(saves, true);
        File.WriteAllText(path, json);
    }

    public void JsonLoad()
    {
        saves = new WeaponProficiencySave();

        if (!File.Exists(path))
        {
            for (int i = 0; i < weaponSO.weapons.Length; i++)
            {
                saves.weaponName.Add(weaponSO.weapons[i].name);
                saves.weaponProficiencyLv.Add(0);
                saves.weaponProficiencyProgress.Add(0f);
            }

            var json = JsonUtility.ToJson(saves, true);
            File.WriteAllText(path, json);
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saves = JsonUtility.FromJson<WeaponProficiencySave>(loadJson);
        }
    }

    public int GetWeaponProficiencyLv(string weaponName)
    {
        for(int i = 0; i < saves.weaponName.Count; i++)
        {
            if(saves.weaponName[i] == weaponName)
            {
                return saves.weaponProficiencyLv[i];
            }
        }

        return -1;
    }

    public float GetWeaponProficiencyValue(string weaponName)
    {
        for (int i = 0; i < saves.weaponName.Count; i++)
        {
            if (saves.weaponName[i] == weaponName)
            {
                return saves.weaponProficiencyProgress[i];
            }
        }

        return -1f;
    }

    public void IncreaseWeaponProficiency(string weaponName, float value)
    {
        for (int i = 0; i < saves.weaponName.Count; i++)
        {
            if (saves.weaponName[i] == weaponName)
            {
                saves.weaponProficiencyProgress[i] += value;

                if(saves.weaponProficiencyProgress[i] > 100)
                {
                    saves.weaponProficiencyLv[i]++;
                    saves.weaponProficiencyProgress[i] = 0f;
                }
            }
        }
    }

    public void Init(WeaponSO weaponSO)
    {
        this.weaponSO = weaponSO;

        JsonLoad();
    }
}