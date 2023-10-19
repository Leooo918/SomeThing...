using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Settings
{
    public bool isCheckWarning = false;
    public float sound = 10f;
}

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance = null;

    private string path = "";

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "Setting.json");

        LoadAll();
    }

    public void SaveAll(float sound, bool isCheckWarning)
    {
        Settings settings = new Settings();

        settings.sound = sound;
        settings.isCheckWarning = isCheckWarning;

        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(path, json);
    }

    public void LoadAll()
    {
        Settings settings = new Settings();

        if (!File.Exists(path))
        {
            SaveAll(settings.sound, settings.isCheckWarning);
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            settings = JsonUtility.FromJson<Settings>(loadJson);

            //셋팅에서 뛰울때 다 로드해주는 식으로만 다른데서는 걍 하나씩 들고오게
        }
    }

    public void SaveCheckWarning(bool isChecked)
    {
        Settings settings = new Settings();

        if (!File.Exists(path))
        {
            SaveAll(settings.sound, isChecked);
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            settings = JsonUtility.FromJson<Settings>(loadJson);

            SaveAll(settings.sound, isChecked);
        }
    }

    public bool LoadCheckWarning()
    {
        Settings settings = new Settings();

        if (!File.Exists(path))
        {
            SaveAll(settings.sound, settings.isCheckWarning);
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            settings = JsonUtility.FromJson<Settings>(loadJson);
        }

        return settings.isCheckWarning;
    }
}
