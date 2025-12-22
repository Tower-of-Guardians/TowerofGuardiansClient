using UnityEngine;

// CSV의 한 행에 대응하는 데이터 구조
[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string Id;
    public string Name;
    public Sprite Image;
    public int HP;
    public int Kind;
    public int PatternType;
    public string Passive1ID;
    public int Passive1Value;
    public string Passive2ID;
    public int Passive2Value;
    public string Passive3ID;
    public int Passive3Value;
    public string Action1ID;
    public int Action1Min;
    public int Action1Max;
    public string Action2ID;
    public int Action2Min;
    public int Action2Max;
    public string Action3ID;
    public int Action3Min;
    public int Action3Max;
    public string Action4ID;
    public int Action4Min;
    public int Action4Max;
    public string Action5ID;
    public int Action5Min;
    public int Action5Max;
    public string Action6ID;
    public int Action6Min;
    public int Action6Max;
    public string Action7ID;
    public int Action7Min;
    public int Action7Max;
}