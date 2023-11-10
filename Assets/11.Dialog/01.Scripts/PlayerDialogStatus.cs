using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Save
{
    public string name;
    public float firstLikeability;
    public float secondLikeability;
    public int curProgress;
}

public class PlayerDialogStatus : MonoBehaviour
{
    private string path = "";

    private string playerName;

    public float firstLikeability { get; [SerializeField]private set; }
    public float secondLikeablity { get; [SerializeField]private set; }
    public int curProgress;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, $"SaveFile/AutoSave.json");
    }

    public void LikeablilityUp(Charactor charactor, float upValue)
    {
        switch (charactor)
        {
            case Charactor.Amelia:
                firstLikeability += upValue;
                break;
            case Charactor.Suba:
                secondLikeablity += upValue;
                break;
        }
    }
    public void LikeablilityDown(Charactor charactor, float downValue)
    {
        switch (charactor)
        {
            case Charactor.Amelia:
                firstLikeability -= downValue;
                break;
            case Charactor.Suba:
                secondLikeablity -= downValue;
                break;
        }
    }

    public void AutoSave()
    {
        path = Path.Combine(Application.dataPath, $"SaveFile/AutoSave.json");
        Save saves = new Save();

        saves.name = playerName;
        saves.firstLikeability = firstLikeability;
        saves.secondLikeability = secondLikeablity;
        saves.curProgress = curProgress;

        string json = JsonUtility.ToJson(saves, true);
        File.WriteAllText(path, json);
    }

    public void AutoLoad()
    {
        path = Path.Combine(Application.dataPath, $"SaveFile/AutoSave.json");
        Save saves = new Save();

        if (!File.Exists(path))
        {
            AutoSave();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saves = JsonUtility.FromJson<Save>(loadJson);

            playerName = saves.name;
            firstLikeability = saves.firstLikeability;
            secondLikeablity = saves.secondLikeability;
            curProgress = saves.curProgress;
        }
    }

    public void MaualSave(int slotNum)
    {
        string slotPath = Path.Combine(Application.dataPath, $"SaveFile/Save{slotNum}.json");
        Save saves = new Save();

        saves.name = playerName;
        saves.firstLikeability = firstLikeability;
        saves.secondLikeability = secondLikeablity;
        saves.curProgress = curProgress;

        string json = JsonUtility.ToJson(saves, true);
        File.WriteAllText(path, json);
    }

    public bool ManualLoad(int slotNum)
    {
        string slotPath = Path.Combine(Application.dataPath, $"SaveFile/Save{slotNum}.json");
        Save saves = new Save();

        if (!File.Exists(slotPath))
        {
            return false;
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saves = JsonUtility.FromJson<Save>(loadJson);

            playerName = saves.name;
            firstLikeability = saves.firstLikeability;
            secondLikeablity = saves.secondLikeability;
            curProgress = saves.curProgress;
            return true;
        }
    }
}
