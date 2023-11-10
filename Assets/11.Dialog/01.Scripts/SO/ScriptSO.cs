using UnityEngine;

public enum ScriptType
{
    NORMAL = 0,
    BRANCH = 1,
    OPTION = 2,
    IMAGE = 3
}


public abstract class ScriptSO : ScriptableObject
{
    public string talkingPeopleName;
    public string talkingDetails;
    public GameObject image;
    public Sprite background;
    public CharacterAnimation anim;

    [Header("---호감도 관련---")]
    public Charactor charactor;
    public float changeLikeyValue;

    [HideInInspector] public string guid;
    [HideInInspector] public ScriptType scriptType;
    [HideInInspector] public Vector2 position;
}
